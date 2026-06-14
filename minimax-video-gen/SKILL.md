---
name: minimax-video-gen
description: 使用 MiniMax 海螺 API 生成视频。当用户提供一张图片，要求让图片中的人物、动物、植物或其他物体做某个动作时，激活此技能。触发词：做一个视频、生成视频、让照片动起来、让图片动起来、视频生成。
version: 1.0.0
authors:
  - OpenClaw Agent
credentials:
  - name: MINIMAX_API_KEY
    required: true
    description: "MiniMax 平台 API Key"
    storage: "systemd service environment variable"
  - name: ALIYUN_OSS_ACCESS_KEY_ID
    required: true
    description: "阿里云 OSS Access Key ID"
    storage: "systemd service environment variable"
  - name: ALIYUN_OSS_ACCESS_KEY_SECRET
    required: true
    description: "阿里云 OSS Access Key Secret"
    storage: "systemd service environment variable"
  - name: ALIYUN_OSS_ENDPOINT
    required: true
    description: "阿里云 OSS Endpoint"
    storage: "systemd service environment variable"
  - name: ALIYUN_OSS_BUCKET
    required: true
    description: "阿里云 OSS Bucket 名称"
    storage: "systemd service environment variable"
---

## Overview

MiniMax Video Generation 是一个基于 MiniMax 海螺 API 的视频生成技能。它能够根据用户提供的静态图片和文字描述，生成一段动态视频。

**核心能力：** 图生视频（Image-to-Video）

## Trigger

此技能应在以下场景激活：

1. **视频生成请求** — 用户说"做一个视频"、"生成视频"、"视频生成"
2. **图片动态化** — 用户说"让照片动起来"、"让图片动起来"
3. **动作描述** — 用户描述图片中的主体（人/动物/植物/物体）应该做什么动作
4. **示例触发句式：**
   - "做一个视频，让照片中的猫咪亲一下小女孩的脸颊"
   - "让这张图片里的女孩跳一支舞"
   - "生成视频，让画面中的狗跑起来"
   - "让图片里的花开放"

## Workflow

### 完整流程

```
用户图片 + 动作描述
    ↓
步骤 1: 上传图片到阿里云 OSS
    ↓
步骤 2: 生成 OSS 签名 URL（1小时有效）
    ↓
步骤 3: 调用 MiniMax 视频生成 API
    - API: POST https://api.minimaxi.com/v1/video_generation
    - 参数: prompt, first_frame_image, model, duration, resolution
    - 返回: task_id
    ↓
步骤 4: 轮询任务状态
    - API: GET https://api.minimaxi.com/v1/query/video_generation?task_id=xxx
    - 等待 status=Success，获取 file_id
    ↓
步骤 5: 获取视频下载链接
    - API: GET https://api.minimaxi.com/v1/files/retrieve?file_id=xxx
    - 返回: file.download_url
    ↓
步骤 6: 下载视频到本地
    ↓
步骤 7: 上传视频到 OSS 并生成签名链接
    ↓
返回给用户签名链接
```

### 环境变量要求

此技能需要以下环境变量（已配置在 systemd service 中）：

| 变量名 | 说明 | 示例值 |
|--------|------|--------|
| `MINIMAX_API_KEY` | MiniMax API Key | `sk-api...xxx` |
| `ALIYUN_OSS_ENDPOINT` | OSS 端点 | `oss-cn-hangzhou.aliyuncs.com` |
| `ALIYUN_OSS_BUCKET` | OSS Bucket | `jackshang` |
| `ALIYUN_OSS_ACCESS_KEY_ID` | OSS Access Key ID | `LTAI5t...xxx` |
| `ALIYUN_OSS_ACCESS_KEY_SECRET` | OSS Access Key Secret | `xxx...xxx` |

### 脚本调用

```bash
python3 <skill_dir>/scripts/minimax_video_gen.py <图片路径> "<动作描述>"
```

**参数说明：**
- `<图片路径>`: 本地图片文件路径（支持 jpg/png）
- `<动作描述>`: 用中文描述希望图片中的主体做什么动作

**示例：**
```bash
python3 <skill_dir>/scripts/minimax_video_gen.py /path/to/photo.jpg "让画面中的猫咪轻轻亲一下小女孩的脸颊"
```

### 输出

脚本会：
1. 上传图片到 OSS
2. 调用 MiniMax API 生成视频
3. 等待生成完成（通常 1-3 分钟）
4. 下载视频并上传到 OSS
5. 输出签名链接（1小时有效）

### 错误处理

| 错误 | 原因 | 解决方案 |
|------|------|---------|
| `MINIMAX_API_KEY not set` | 环境变量未配置 | 检查 systemd service 配置 |
| `Invalid API-key` | API Key 无效 | 检查 MiniMax 账户状态 |
| `first_frame_image: failed to download image` | 图片 URL 无法访问 | 确认 OSS 签名链接有效 |
| `Task timeout` | 视频生成超时 | 重试或联系 MiniMax 支持 |

## Models

支持的 MiniMax 视频模型：

| 模型 | 说明 | 推荐场景 |
|------|------|---------|
| `I2V-01` | 图生视频基础版 | 通用场景 |
| `I2V-01-Director` | 图生视频导演版 | 需要更精确控制 |
| `I2V-01-live` | 图生视频直播版 | 实时风格 |
| `MiniMax-Hailuo-2.3` | 文生视频 | 无参考图时使用 |

## Limitations

- 图片大小限制：建议小于 10MB
- 视频时长：6-10 秒
- 视频分辨率：720P / 1080P
- 生成时间：约 1-3 分钟
- 签名链接有效期：1 小时

## Security Notes

- API Key 存储在 systemd service 环境变量中，不在日志中输出
- OSS 签名链接有时效性，过期需重新生成
- 视频文件存储在用户 OSS Bucket 中，需注意访问权限
