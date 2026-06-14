---

name: audio-transcription
description: Audio/video speech-to-text transcription using Alibaba Cloud DashScope ASR APIs. Use when transcribing video or audio content, extracting spoken text from recordings, analyzing speech in media files, or converting voice notes to text. Supports MP4, MP3, WAV and other formats.
---

# Audio Transcription

Convert spoken content in audio/video files to text using Alibaba Cloud DashScope ASR APIs.

## 价格参考

**阿里云 DashScope 语音识别：**

* 异步识别（录音文件）：约 **0.00022元/秒**

**成本示例：**

|时长|成本|
|-|-|
|1分钟|¥0.013|
|10分钟|¥0.13|
|1小时|¥0.79|

## Quick Start

```python
# Execute the transcription script
python3 \~/.openclaw/skills/audio-transcription/scripts/asr\_transcribe.py <audio\_or\_video\_file>

# Example: transcribe a video
python3 \~/.openclaw/skills/audio-transcription/scripts/asr\_transcribe.py videos/meeting.mp4

# Example: transcribe an audio file
python3 \~/.openclaw/skills/audio-transcription/scripts/asr\_transcribe.py audio/voice\_note.mp3

# Save result to JSON file
python3 \~/.openclaw/skills/audio-transcription/scripts/asr\_transcribe.py video.mp4 -o result.json
```

## How It Works

The skill automatically selects the optimal method based on audio duration:

|Duration|Method|Model|Characteristics|
|-|-|-|-|
|**≤5 min**|SDK (qwen3-asr-flash)|qwen3-asr-flash|Fast, no upload needed, streaming output|
|**>5 min**|Async API (paraformer-v2)|paraformer-v2|Supports longer audio, requires upload|

## Workflow

1. **Detect file type** - If video (MP4, etc.), extract audio to MP3
2. **Check duration** - Determine optimal transcription method
3. **Execute transcription**:

   * Short audio: Use SDK with `file:///` local path
   * Long audio: Upload to DashScope Files API → submit async task → poll for result
4. **Return result** - Text + sentence-level timestamps

## Output Format

```json
{
  "text": "Full transcription text",
  "sentences": \[
    {"begin\_time": 0, "end\_time": 1680, "text": "Sentence 1"},
    {"begin\_time": 1680, "end\_time": 3500, "text": "Sentence 2"}
  ],
  "model": "paraformer-v2",
  "method": "async",
  "audio\_info": {"format": "mp3", "sample\_rate": 22050}
}
```

## Supported Formats

* **Video**: MP4, MOV, AVI, MKV (audio extracted automatically)
* **Audio**: MP3, WAV, FLAC, AAC, OGG, M4A

## API Configuration

The skill reads DashScope API key from:

1. **Environment variable** (优先): `ALIBABA_API_KEY`
2. OpenClaw config (兼容旧配置): `/root/.openclaw/openclaw.json` → `models.providers.alibaba.apiKey`

> **注意**: 从 2026-05-27 开始，配置已改为从环境变量获取 API Key，openclaw.json 中的配置为 `${ALIBABA_API_KEY}`

## Manual Method Selection

Force a specific model if needed:

```bash
# Use SDK method (fast, for short audio)
python3 \~/.openclaw/skills/audio-transcription/scripts/asr\_transcribe.py audio.mp3 --model qwen3-asr-flash

# Use async API (for long audio)
python3 \~/.openclaw/skills/audio-transcription/scripts/asr\_transcribe.py audio.mp3 --model paraformer-v2
```

## Requirements

* **ffmpeg** - For audio extraction from video files
* **dashscope** (Python SDK) - For SDK transcription method
* **requests** - For async API method

Install dependencies:

```bash
pip install dashscope requests
apt install ffmpeg  # or brew install ffmpeg on macOS
```

## Resources

### scripts/asr\_transcribe.py

Main transcription script that:

* Auto-detects audio/video format
* Extracts audio from video if needed
* Selects optimal ASR method based on duration
* Handles file upload for async API
* Returns structured transcription result with timestamps

