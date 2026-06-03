#!/usr/bin/env node

/**
 * 小红书笔记发布脚本 v2
 * 
 * 用法：
 *   NODE_PATH=$(npm root -g):/root/.openclaw/workspace/node_modules node publish.js \
 *     --images "/path/to/img1.png,/path/to/img2.png" \
 *     --title "标题（≤20字）" \
 *     --content "正文内容"
 * 
 * 参数：
 *   --images    图片路径，多个用逗号分隔（必填）
 *   --title     笔记标题，≤20个汉字（必填）
 *   --content   笔记正文，含话题标签（必填）
 *   --cdpUrl    CDP地址，默认 http://127.0.0.1:9222
 *   --timeout   超时时间(ms)，默认 60000
 * 
 * 返回：
 *   JSON { success: boolean, message: string, url?: string }
 * 
 * 注意：必须用 NODE_PATH 指定 node_modules 路径，否则找不到 puppeteer-core
 */

const puppeteer = require('puppeteer-core');
const path = require('path');
const fs = require('fs');

// 解析命令行参数
function parseArgs() {
  const args = process.argv.slice(2);
  const params = {
    images: null,
    title: null,
    content: null,
    cdpUrl: 'http://127.0.0.1:9222',
    timeout: 60000
  };

  for (let i = 0; i < args.length; i++) {
    if (args[i] === '--images' && args[i + 1]) {
      params.images = args[++i].split(',').map(p => p.trim());
    } else if (args[i] === '--title' && args[i + 1]) {
      params.title = args[++i];
    } else if (args[i] === '--content' && args[i + 1]) {
      params.content = args[++i];
    } else if (args[i] === '--cdpUrl' && args[i + 1]) {
      params.cdpUrl = args[++i];
    } else if (args[i] === '--timeout' && args[i + 1]) {
      params.timeout = parseInt(args[++i], 10);
    }
  }

  return params;
}

// 校验参数
function validate(params) {
  if (!params.images || params.images.length === 0) {
    return { valid: false, error: '缺少 --images 参数' };
  }
  if (params.images.length > 18) {
    return { valid: false, error: `图片超过18张（当前${params.images.length}张）` };
  }
  if (!params.title) {
    return { valid: false, error: '缺少 --title 参数' };
  }
  if (params.title.length > 20) {
    return { valid: false, error: `标题超过20字（当前${params.title.length}字）` };
  }
  if (!params.content) {
    return { valid: false, error: '缺少 --content 参数' };
  }

  for (const img of params.images) {
    if (!fs.existsSync(img)) {
      return { valid: false, error: `图片文件不存在: ${img}` };
    }
  }

  return { valid: true };
}

function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

// 将正文转换成 TipTap 编辑器可接受的 HTML（分段）
function contentToHtml(text) {
  const lines = text.split('\n');
  const paragraphs = lines.map(line => {
    const trimmed = line.trim();
    if (trimmed === '') return '<p></p>';
    return `<p>${escapeHtml(trimmed)}</p>`;
  });
  return paragraphs.join('\n');
}

function escapeHtml(text) {
  return text
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');
}

// 主发布流程
async function publish(params) {
  let browser;
  
  try {
    browser = await puppeteer.connect({
      browserURL: params.cdpUrl,
      defaultViewport: null
    });

    // 打开发布页面
    const page = await browser.newPage();
    await page.goto('https://creator.xiaohongshu.com/publish/publish', {
      waitUntil: 'networkidle2',
      timeout: params.timeout
    });
    await sleep(2000);

    // 切换到图文模式
    await page.evaluate(() => {
      const tabs = document.querySelectorAll('.creator-tab');
      for (const t of tabs) {
        if (t.textContent.includes('上传图文') && !t.className.includes('active')) {
          t.click();
          return;
        }
      }
    });
    await sleep(3000);

    // 用 CDP 一次性上传所有图片
    // 注意：不能逐张上传，小红书只接受最后一张
    // 必须一次性传入所有文件路径
    const cdp = await page.createCDPSession();
    const doc = await cdp.send('DOM.getDocument');
    const fileInput = await cdp.send('DOM.querySelector', {
      nodeId: doc.root.nodeId,
      selector: 'input[type="file"]'
    });
    await cdp.send('DOM.setFileInputFiles', {
      nodeId: fileInput.nodeId,
      files: params.images
    });
    console.error(`[upload] ${params.images.length} files sent`);

    // 等待页面处理图片（约5-8秒）
    await sleep(6000);

    // 填写标题
    await page.evaluate((title) => {
      const input = document.querySelector('input[placeholder="填写标题会有更多赞哦"]');
      if (input) {
        input.value = title;
        input.dispatchEvent(new Event('input', { bubbles: true }));
        input.dispatchEvent(new Event('change', { bubbles: true }));
      }
    }, params.title);
    console.error('[title] set');

    // 填写正文 - 用 ClipboardEvent paste 注入 HTML，保留换行
    await page.evaluate((html) => {
      const editor = document.querySelector('.tiptap.ProseMirror');
      if (!editor) return;
      
      // 先清空
      editor.innerHTML = '<p><br></p>';
      
      // 通过粘贴事件注入 HTML
      const clipboardData = new DataTransfer();
      clipboardData.setData('text/html', html);
      clipboardData.setData('text/plain', '');
      
      const pasteEvent = new ClipboardEvent('paste', {
        clipboardData,
        bubbles: true,
        cancelable: true
      });
      
      editor.dispatchEvent(pasteEvent);
    }, contentToHtml(params.content));
    console.error('[content] set');
    await sleep(1000);

    // 移除遮罩层
    await page.evaluate(() => {
      const all = document.querySelectorAll('*');
      for (let i = 0; i < all.length; i++) {
        const s = getComputedStyle(all[i]);
        if (s.position === 'fixed' && parseInt(s.zIndex) > 90000) {
          all[i].style.display = 'none';
        }
      }
    });
    await sleep(500);

    // 点击发布按钮
    const publishResult = await page.evaluate(() => {
      const btn = document.querySelector('xhs-publish-btn');
      if (!btn) {
        return { success: false, error: '找不到发布按钮 xhs-publish-btn' };
      }
      if (typeof btn._onPublish === 'function') {
        btn._onPublish();
        return { success: true, method: '_onPublish' };
      }
      return { success: false, error: '_onPublish 方法不存在' };
    });

    if (!publishResult.success) {
      return { success: false, message: publishResult.error };
    }

    // 等待发布结果
    await sleep(5000);

    // 检测发布结果
    const result = await page.evaluate(() => {
      const url = window.location.href;
      const bodyText = document.body.innerText;

      // 判断发布成功（URL 跳转到 publish/success）
      if (url.includes('/publish/success')) {
        return { success: true, message: '发布成功', url };
      }

      // 检查错误提示
      if (bodyText.includes('发布失败') || bodyText.includes('错误')) {
        return { success: false, message: '发布失败，页面有错误提示', url };
      }

      return { success: true, message: '已提交发布请求', url };
    });

    await page.close();
    return result;

  } catch (error) {
    return {
      success: false,
      message: `发布失败: ${error.message}`
    };
  } finally {
    if (browser) {
      browser.disconnect();
    }
  }
}

// 带重试的发布
async function publishWithRetry(params, maxRetries = 1) {
  let lastError;
  
  for (let attempt = 0; attempt <= maxRetries; attempt++) {
    if (attempt > 0) {
      console.error(`[retry ${attempt + 1}/${maxRetries}] 重试...`);
      await sleep(3000);
    }
    
    const result = await publish(params);
    
    if (result.success) {
      return result;
    }
    
    // 不可重试的错
    if (result.message.includes('图片文件不存在') ||
        result.message.includes('标题超过20字')) {
      return result;
    }
    
    lastError = result;
  }
  
  return {
    success: false,
    message: `${lastError.message}（已重试 ${maxRetries} 次）`
  };
}

async function main() {
  const params = parseArgs();
  
  const validation = validate(params);
  if (!validation.valid) {
    console.log(JSON.stringify({ success: false, message: validation.error }));
    process.exit(1);
  }

  const result = await publishWithRetry(params, 1);
  console.log(JSON.stringify(result));
  process.exit(result.success ? 0 : 1);
}

main();
