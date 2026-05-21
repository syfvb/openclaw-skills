---
name: video-summarizer
description: Comprehensive video content analysis combining keyframe extraction, image recognition, and speech-to-text transcription. Use when summarizing video content, analyzing video presentations, extracting information from video recordings, or understanding what happens in a video file. Provides visual and audio dimensions for complete video understanding.
---

# Video Summarizer

Extract keyframes and transcribe audio from video files, then synthesize both dimensions into a comprehensive summary.

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
Video File
    │
    ├── video_summarize.py
    │   ├── Extract keyframes (auto interval based on duration)
    │   ├── Extract audio
    │   └── Transcribe speech:
    │       ├── 1️⃣ Try FunASR skill (local, offline)
    │       └── 2️️ If fails → audio-transcription skill (cloud API)
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