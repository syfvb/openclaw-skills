---
name: video-summarizer
description: Comprehensive video content analysis combining keyframe extraction, image recognition, and speech-to-text transcription. Use when summarizing video content, analyzing video presentations, extracting information from video recordings, or understanding what happens in a video file. Provides visual and audio dimensions for complete video understanding.
---

# Video Summarizer

Extract keyframes and transcribe audio from video files, then synthesize both dimensions into a comprehensive summary.

## URL Preprocessing

When input is a URL instead of a local file, **download first, then analyze**.

| Source | Tool | Proxy | Notes |
|:---|:---|:---|:---|
| **YouTube** | `yt-dlp` | **Required** | `proxy on` first, then `yt-dlp -o <output> <url>` |
| **Bilibili** | `yt-dlp` | No | Direct download, no proxy needed |
| **Toutiao** | Playwright + ffmpeg | No | Blob URL streaming, must use browser interceptor, **not** yt-dlp (see [Toutiao Download](#toutiao-download)) |
| Local file | — | — | Skip download, go straight to analysis |

### Download commands

```bash
# YouTube (must enable proxy first)
proxy on
yt-dlp -o "/tmp/video.%(ext)s" "<youtube_url>"

# Bilibili (no proxy)
yt-dlp -o "/tmp/video.%(ext)s" "<bilibili_url>"

# Toutiao — see Toutiao Download section below
```

After download, pass the local file path to the analysis script below.

## Quick Start

### Step 1: Analyze Video (Extract Data)

```bash
# Extract keyframes and audio transcript
python3 ~/.openclaw/skills/video-summarizer/scripts/video_summarize.py <video_file>

# Example
python3 ~/.openclaw/skills/video-summarizer/scripts/video_summarize.py videos/demo.mp4
```

This creates an `*_analysis` directory containing:
- `frames/` - Extracted keyframe images
- `audio.mp3` - Extracted audio
- `analysis_result.json` - Analysis metadata

### Step 2: Analyze Keyframes (Visual Content)

After script execution, analyze the keyframes using image tool:

```
# In conversation, request:
请分析这些关键帧: videos/demo_analysis/frames/frame_*.jpg

# Or use image tool directly with specific frames
```

### Step 3: Synthesize Summary

Combine visual analysis and audio transcript to produce final summary:

- **Visual dimension**: Scenes, people, objects, UI screens, text overlays
- **Audio dimension**: Spoken content, narration, dialogue
- **Combined**: Complete video content summary

## Workflow

```
Input
  │
  ├── URL?
  │    ├── YouTube → proxy on → yt-dlp → local file
  │    ├── Bilibili → yt-dlp → local file
  │    └── Toutiao → Playwright interceptor → ffmpeg merge → local file
  │
  └── Local file → directly to analysis
        │
        ├── video_summarize.py
        │   ├── Extract keyframes (auto interval based on duration)
        │   ├── Extract audio
        │   └── Transcribe speech:
        │       ├── 1️⃣ Try FunASR skill (local, offline)
        │       └── 2️⃣ If fails → audio-transcription skill (cloud API)
        │   └── Save to analysis_result.json
        │
        ├── image tool (manual step)
        │   └── Analyze keyframes → Visual content
        │
        └── Synthesis (by AI)
            └── Visual + Audio → Complete summary
```

## Speech-to-Text Strategy

| Priority | Skill | Method | Use Case |
|:---|:---|:---|:---|
| **1st** | funasr | Local (Paraformer) | Privacy, offline, Chinese content |
| **2nd** | audio-transcription | Cloud (DashScope) | Network available, multi-language |

## Frame Extraction Strategy

The script automatically adjusts frame interval based on video duration:

| Duration | Interval | Max Frames |
|:---|:---|:---|
| ≤30s | 2 seconds | 15 |
| 30s-2min | 5 seconds | 24 |
| >2min | 10 seconds | 20 |

## Output Structure

```
video_analysis/
├── frames/
│   ├── frame_001.jpg
│   ├── frame_002.jpg
│   └── ...
├── audio.mp3
├── analysis_result.json    # Contains transcript and metadata
```

## Example Result

```json
{
  "video_path": "videos/demo.mp4",
  "video_info": {"duration": 75.5, "width": 1920, "height": 1080},
  "frames": ["videos/demo_analysis/frames/frame_001.jpg", ...],
  "frame_interval": 5,
  "audio_path": "videos/demo_analysis/audio.mp3",
  "transcript": "这是一个产品介绍视频...",
  "output_dir": "videos/demo_analysis"
}
```

## Toutiao Download

今日头条 uses blob URL streaming (DASH format with separate video/audio). yt-dlp does **not** work.

### Trigger

When URL contains `toutiao.com` or user mentions: 头条视频 / 今日头条视频 / 下载头条视频 / 分析头条视频 / 转写头条视频.

⚠️ **必须先读本节再操作**，禁止跳过直接用 curl/yt-dlp。

### Workflow

1. **Open browser** to the Toutiao video page
2. **Inject interceptor** — run JavaScript to capture stream URLs
3. **Play video** to trigger stream requests
4. **Extract URLs** — find video (`media-video-avc1`) and audio (`media-audio-und-mp4a`) streams
5. **Download with ffmpeg** — merge into single MP4

### Interceptor Script

```javascript
// Inject before playing video
window._videoRequests = [];

const origFetch = window.fetch;
window.fetch = function(url, opts) {
  const urlStr = url.toString();
  if (urlStr.includes('.mp4') || urlStr.includes('.m3u8') || 
      urlStr.includes('video') || urlStr.includes('media') || urlStr.includes('.ts')) {
    window._videoRequests.push({ type: 'fetch', url: urlStr });
  }
  return origFetch.apply(this, arguments);
};

const video = document.querySelector('video');
if (video) video.play();
```

### Extract URLs

```javascript
const reqs = window._videoRequests || [];
// Find video stream (media-video-avc1)
// Find audio stream (media-audio-und-mp4a)
// Add https: prefix if URL starts with //
```

### ffmpeg Merge

```bash
ffmpeg -i "$VIDEO_URL" -i "$AUDIO_URL" -c:v copy -c:a aac -strict experimental output.mp4 -y
```

### Notes

- URLs are temporary, download immediately after extraction
- 用 `curl -L --retry 3 --max-time 300` + 正确 User-Agent/Referer
- 30分钟视频可能 50MB+，确保超时设置充足
- 下载后用 `ffprobe` 验证完整时长
- 反爬严格：直接 curl 大概率被 JS 混淆拦截，必须用浏览器方案

## Dependencies

- **ffmpeg** - Video/audio processing
- **funasr skill** - Local offline speech-to-text (优先使用)
- **audio-transcription skill** - Cloud speech-to-text (备用方案)

## Resources

### scripts/video_summarize.py

Main analysis script that:
- Extracts keyframes at adaptive intervals
- Extracts audio from video
- Invokes audio-transcription for speech-to-text
- Outputs structured analysis data

## Usage Example

```
User: 总结 videos/product_demo.mp4 的内容

AI:
1. python3 ~/.openclaw/skills/video-summarizer/scripts/video_summarize.py videos/product_demo.mp4
   → Extracts 15 keyframes, transcribes audio

2. image tool analyzes keyframes
   → UI screenshots, product features, brand logos

3. Synthesizes:
   "这是一个产品演示视频，时长约2分钟。
   视频展示了汉得ChatBI产品的核心功能...
   [结合视觉和语音内容的完整总结]"
```