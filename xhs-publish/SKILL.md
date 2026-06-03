---
name: xhs-publish
description: "自动发布小红书笔记：上传图片、填写标题正文、点击发布。触发词：发布小红书、小红书发布、xhs publish。"
---

# 小红书自动发布 v2

将已创建好的小红书笔记图片自动发布到小红书创作平台。

## 前置条件

- 浏览器已登录小红书创作平台（`https://creator.xiaohongshu.com`）
- 笔记图片已准备好（PNG/JPG 格式，推荐 3:4 比例）
- 标题 ≤ 20 个汉字
- 图片 ≤ 18 张
- 发布脚本需要 `NODE_PATH` 环境变量

## 发布流程

### 步骤 1：调脚本（首选）

```bash
NODE_PATH=$(npm root -g):/root/.openclaw/workspace/node_modules \
  node ~/.openclaw/skills/xhs-publish/scripts/publish.js \
  --images "/tmp/openclaw/uploads/slide-01.png,/tmp/openclaw/uploads/slide-02.png,..." \
  --title "标题（≤20字）" \
  --content "正文内容 #话题标签"
```

**参数说明：**
| 参数 | 必填 | 说明 |
|------|------|------|
| `--images` | ✅ | 图片路径，多个用逗号分隔，建议上传前确保图片在 `/tmp/openclaw/uploads/` |
| `--title` | ✅ | 标题，≤20 个汉字 |
| `--content` | ✅ | 正文，含话题标签 |
| `--cdpUrl` | ❌ | CDP 地址，默认 `http://127.0.0.1:9222` |
| `--timeout` | ❌ | 超时 ms，默认 60000 |

**返回值：** `{"success":true,"message":"发布成功","url":"..."}`

### 步骤 2：脚本失败 → 降级到浏览器工具（限制 3 次操作）

脚本失败时，用浏览器工具手动完成，但限制最多 **3 次操作**。

**操作计价规则：** 截图 + 分析 + 执行动作 = 1 次

## 关键踩坑记录

### 1. 切换图文模式

**问题：** 旧的脚本用 `div[role="tab"]` 查找上传图文按钮，实际不存在。

**解法：** 用 `div.creator-tab`。判断是否激活用 `className.includes('active')`。

```javascript
const tabs = document.querySelectorAll('.creator-tab');
for (const t of tabs) {
  if (t.textContent.includes('上传图文') && !t.className.includes('active')) {
    t.click(); return;
  }
}
```

### 2. 图片上传（最大坑）

**问题：** 小红书有 2 个 file input（视频 + 图片），必须先切换到图文模式再上传。`puppeteer.uploadFile()` 不支持 multiple file input（报错 "Only supports single file upload"）。CDP 逐张上传只保留最后一张。

**解法：** CDP `DOM.setFileInputFiles` **一次性传入所有文件路径**。

```javascript
const cdp = await page.createCDPSession();
const doc = await cdp.send('DOM.getDocument');
const input = await cdp.send('DOM.querySelector', {
  nodeId: doc.root.nodeId,
  selector: 'input[type="file"]'
});
await cdp.send('DOM.setFileInputFiles', {
  nodeId: input.nodeId,
  files: ['img1.png', 'img2.png', 'img3.png']  // 全部一次性传入
});
```

**注意：** 
- ❌ 不要逐张上传（只有最后一张生效）
- ✅ 一次性传入所有文件（全部生效）
- 上传后需要等 5-8 秒让页面处理图片

### 3. 标题输入框

**问题：** `input[placeholder*="标题"]` 选择器找不到，实际 placeholder 是完整文本。

**解法：**
```javascript
const input = document.querySelector('input[placeholder="填写标题会有更多赞哦"]');
input.value = '标题';
input.dispatchEvent(new Event('input', { bubbles: true }));
input.dispatchEvent(new Event('change', { bubbles: true }));
```

### 4. 正文编辑器

**问题：** 编辑器是 TipTap/ProseMirror，`type` 方式输入的换行不生效（Enter 被编辑器捕获），`execCommand('insertText')` 也会丢失换行。

**解法：** 用 ClipboardEvent paste 注入 HTML，保留分段：

```javascript
const editor = document.querySelector('.tiptap.ProseMirror');
const clipboardData = new DataTransfer();
clipboardData.setData('text/html', '<p>段落1</p><p></p><p>段落2</p>');
const pasteEvent = new ClipboardEvent('paste', {
  clipboardData, bubbles: true, cancelable: true
});
editor.dispatchEvent(pasteEvent);
```

### 5. 遮罩层

**问题：** 发布页面有 z-index=99999 的 fixed 全屏遮罩层挡住点击。

**解法：** 发布前先移除：

```javascript
const all = document.querySelectorAll('*');
for (let i = 0; i < all.length; i++) {
  const s = getComputedStyle(all[i]);
  if (s.position === 'fixed' && parseInt(s.zIndex) > 90000) {
    all[i].style.display = 'none';
  }
}
```

### 6. 发布按钮

**问题：** `xhs-publish-btn` 是 Web Component，坐标点击、PointerEvent 注入都不可靠。

**解法：** 直接调组件内部方法 `_onPublish()`，100% 可靠。

```javascript
const btn = document.querySelector('xhs-publish-btn');
btn._onPublish();
```

**检测按钮存在：**
```javascript
const btn = document.querySelector('xhs-publish-btn');
// 检查暴露的方法
Object.keys(btn).filter(k => !k.startsWith('__'));
// 会看到 ['_sr', '_app', '_props', '_onPublish', '_onSave']
```

## 完整发布时序

```
1. 连接浏览器（puppeteer.connect）
2. 打开 https://creator.xiaohongshu.com/publish/publish
3. wait 2s 等待页面加载
4. 点击 div.creator-tab（内容含"上传图文"，不含"active"）
5. wait 3s 切换到图文模式
6. CDP DOM.setFileInputFiles 一次性上传所有图片
7. wait 6s 等待图片处理
8. 设置标题 input（触发 input + change 事件）
9. ClipboardEvent paste 注入正文 HTML
10. wait 1s
11. 移除遮罩层（z-index > 90000 的 fixed 元素隐藏）
12. btn._onPublish()
13. wait 5s
14. 检查 URL 是否跳转到 /publish/success
15. 返回结果
```

## 失败处理流程

```
脚本失败 → 判断可重试 → 重试 1 次 → 仍失败 → 降级浏览器工具 → 限制 3 次操作 → 超限报错
```

**可重试的错误：** 页面加载超时、网络波动、组件渲染延迟
**不可重试的错误：** 图片文件不存在、标题超长

## 错误处理

| 错误信息 | 原因 | 处理 |
|----------|------|------|
| 图片文件不存在 | 路径错误 | 检查路径，重新调脚本 |
| 标题超过20字 | 标题 > 20 汉字 | 缩短标题，重新调脚本 |
| 找不到发布按钮 xhs-publish-btn | 页面未加载/组件不存在 | 降级浏览器工具 |
| 发布失败 | CDP/setContent 报错 | 降级浏览器工具 |
| 浏览器连接失败 | CDP 端口不对/浏览器未运行 | 检查浏览器状态，提醒用户 |

## NODE_PATH 说明

由于 publish.js 在 `~/.openclaw/skills/` 目录下运行，而 puppeteer-core 安装在 workspace 和全局，执行时必须指定 NODE_PATH：

```bash
NODE_PATH=$(npm root -g):/root/.openclaw/workspace/node_modules node publish.js ...
```

## 禁止事项

1. ❌ 禁止用浏览器工具反复尝试发布 — 先调脚本
2. ❌ 禁止浏览器工具操作超过 3 次 — 超过直接报错
3. ❌ 禁止死循环 — 3 次是硬上限
