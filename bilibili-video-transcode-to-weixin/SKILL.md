---
name: bilibili-video-transcode-to-weixin
description: B站视频兼容性修复。将B站下载的AV1/HEVC视频转码为微信/花瓣等手机软件兼容的H.264 MP4，并支持zip打包传输。触发词：B站视频、bilibili视频、视频发不了、视频保存失败、花瓣打不开、视频转码。
version: 1.0.0
authors:
  - openclaw
---

# B站视频兼容性修复

## 问题背景

B站（bilibili）下载的视频通常是 AV1 编码的 MP4 文件，存在以下兼容性问题：

| 问题 | 原因 |
|------|------|
| 微信无法作为视频发送 | 微信只支持 H.264 编码 |
| 微信发送后无法保存 | 微信转码链路对非标准参数容错差 |
| 花瓣/剪映等剪辑软件闪退 | 无法解析 AV1 编码或异常 SAR |
| 电脑能播手机不行 | 电脑播放器容错强，手机端严格 |

## 核心参数

B站下载视频的典型特征：
- 编码：AV1（av01）或 H.265（HEVC）
- 容器：MP4
- SAR 异常：640:639（B站特有）
- 元数据：含 `Bilibili VXCode Swarm Transcoder` 标记

## 转码方案

### 标准转码（发送/编辑用）

```bash
ffmpeg -i input.mp4 \
  -c:v libx264 -profile:v baseline -level 3.1 \
  -pix_fmt yuv420p \
  -colorspace bt709 -color_primaries bt709 -color_trc bt709 \
  -vf "setsar=1:1" \
  -c:a aac -ac 2 -ar 44100 -b:a 96k \
  -movflags +faststart \
  -map_metadata -1 \
  -f mp4 output.mp4
```

参数说明：
| 参数 | 值 | 说明 |
|------|-----|------|
| profile | baseline | 兼容性最广，所有设备支持 |
| level | 3.1 | 支持 720p@30fps，安全范围 |
| SAR | 1:1 | 修复B站异常像素比 |
| faststart | ✓ | moov atom 置顶，微信能快速识别 |
| map_metadata | -1 | 清除B站专有元数据，避免干扰 |
| colorspace | bt709 | 标准色彩空间 |
| audio | AAC 44100Hz 96kbps | 微信标准音频参数 |

### 低码率版（文件更小）

```bash
ffmpeg -i input.mp4 \
  -c:v libx264 -preset slow -crf 28 \
  -profile:v baseline -level 3.0 \
  -pix_fmt yuv420p -vf "scale=640:360,setsar=1:1" \
  -c:a aac -ac 2 -ar 44100 -b:a 64k \
  -movflags +faststart \
  -map_metadata -1 \
  -f mp4 output_low.mp4
```

## 微信传输方案

### 方案一：zip 打包（推荐，100% 可保存）

转码后 zip 打包发送，完全绕过微信视频处理链路：

```bash
zip output.zip output.mp4
```

接收方解压即可得到标准 MP4，微信/花瓣/剪映全能用。

### 方案二：文件形式发送

直接以文件（非视频消息）形式发送，微信不做转码。但保存仍可能失败，取决于微信版本和设备。

### 方案三：视频消息

直接作为视频发送，微信会重新转码。格式正确时可以播放和保存，但部分设备仍可能保存失败。

## 一键脚本

```bash
bash ~/.openclaw/skills/bilibili-video-transcode-to-weixin/scripts/fix.sh <input.mp4> [output_name]
```

示例：
```bash
bash ~/.openclaw/skills/bilibili-video-transcode-to-weixin/scripts/fix.sh video.mp4 my_video
# 输出：my_video_fixed.mp4 + my_video_fixed.zip
```

## 使用场景

1. **B站视频发微信** → 转码 + zip 打包
2. **B站视频用花瓣/剪映编辑** → 转码即可
3. **B站视频存手机相册** → 转码 + zip 解压
4. **批量处理** → 用脚本循环处理多个文件

## 注意事项

- 转码后文件会变大（H.264 压缩效率不如 AV1），属正常现象
- Baseline Profile 不支持 B 帧和 CABAC，压缩率低但兼容性最强
- 如果只需要播放不需要保存，直接发视频消息即可
- zip 打包是微信传输最可靠的方式，推荐作为默认方案
