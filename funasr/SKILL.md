---
name: funasr
description: Local offline audio/video speech-to-text transcription using FunASR (Paraformer). Use when transcribing audio/video content locally without cloud API, privacy-sensitive content, batch processing, or when DashScope is unavailable. Supports Chinese, English, and mixed languages with high accuracy.
---

# FunASR - Local ASR Transcription

Convert spoken content in audio/video files to text using Alibaba's FunASR framework (Paraformer) running locally.

## Key Advantages

| Feature | DashScope (Cloud) | FunASR (Local) |
|:---|:---|:---|
| **Privacy** | Data uploaded to cloud | ✅ Data stays local |
| **Cost** | Pay per use | ✅ Free after setup |
| **Speed** | API latency | ✅ ~12x realtime (CPU) |
| **Accuracy (Chinese)** | Good | ✅ Better (Paraformer) |
| **Network** | Required | ✅ Works offline |

## Quick Start

```bash
# Basic transcription
python3 ~/.openclaw/skills/funasr/scripts/funasr_transcribe.py <audio_or_video_file>

# Example: transcribe a video
python3 ~/.openclaw/skills/funasr/scripts/funasr_transcribe.py videos/meeting.mp4

# Example: transcribe an audio file
python3 ~/.openclaw/skills/funasr/scripts/funasr_transcribe.py audio/voice_note.mp3

# Save result to JSON file
python3 ~/.openclaw/skills/funasr/scripts/funasr_transcribe.py video.mp4 -o result.json

# Use streaming model (faster startup)
python3 ~/.openclaw/skills/funasr/scripts/funasr_transcribe.py audio.mp3 --model streaming

# Show detailed progress
python3 ~/.openclaw/skills/funasr/scripts/funasr_transcribe.py video.mp4 --verbose
```

## Models

| Model | Use Case | Characteristics |
|:---|:---|:---|
| **paraformer-zh** (default) | Offline transcription | High accuracy, sentence-level timestamps |
| **paraformer-zh-streaming** | Low latency | Faster, chunk-based processing |

## Workflow

1. **Check dependencies** - Verify ffmpeg, funasr, torch installed
2. **Extract audio** - If video, extract to WAV/MP3
3. **Load model** - First run downloads model (~950MB), cached afterwards
4. **Transcribe** - Paraformer processes audio locally
5. **Return result** - Text + timestamps + performance metrics

## Output Format

```json
{
  "text": "完整转录文本",
  "sentences": [
    {"begin_time": 0, "end_time": 1680, "text": "句子1"},
    {"begin_time": 1680, "end_time": 3500, "text": "句子2"}
  ],
  "model": "paraformer-zh",
  "audio_info": {"duration": 1521, "format": "wav", "sample_rate": 16000},
  "performance": {
    "load_time_sec": 106.4,
    "transcribe_time_sec": 133.6,
    "rtf": 0.079,
    "speed_factor": 11.4
  }
}
```

**RTF (Real-Time Factor)**: 0.079 means processing 1 second of audio takes 0.079 seconds (12x realtime).

## Supported Formats

- **Video**: MP4, MOV, AVI, MKV, WebM (audio extracted automatically)
- **Audio**: MP3, WAV, FLAC, AAC, OGG, M4A

## Requirements

```bash
# Install Python dependencies
pip install funasr modelscope torch torchaudio

# Install ffmpeg (for video extraction)
apt install ffmpeg  # Linux
brew install ffmpeg  # macOS
```

**First Run**: Models are downloaded automatically to `~/.cache/modelscope/` (~1GB total).

## Performance Tips

| Tip | Effect |
|:---|:---|
| **Use streaming model** | Faster startup, slightly lower accuracy |
| **GPU available** | Add `--device cuda` for 5-10x speedup |
| **Batch processing** | Process multiple files sequentially |
| **Keep model loaded** | Reuse process for multiple transcriptions |

## Comparison with DashScope Skill

Use **FunASR** when:
- Privacy-sensitive content
- No internet or unreliable connection
- Batch processing many files
- Chinese audio with technical terms

Use **DashScope** when:
- Quick one-off transcription
- Multi-language content (not Chinese)
- No local GPU/CPU resources

## Error Handling

| Error | Cause | Solution |
|:---|:---|:---|
| `ModuleNotFoundError: funasr` | FunASR not installed | `pip install funasr modelscope` |
| `No audio detected` | Silent or corrupt file | Check audio quality |
| `Out of memory` | Long audio + low RAM | Use streaming model |

## Resources

### scripts/funasr_transcribe.py

Main transcription script with:
- Auto dependency check
- Audio extraction from video
- Model selection (offline/streaming)
- Progress reporting
- JSON output with performance metrics

### Model Cache Location

Models are cached at:
- `~/.cache/modelscope/hub/models/iic/speech_paraformer-large_asr_nat-zh-cn-16k-common-vocab8404-pytorch/`
- `~/.cache/modelscope/hub/models/iic/speech_fsmn_vad_zh-cn-16k-common-pytorch/` (VAD)
- `~/.cache/modelscope/hub/models/iic/punc_ct-transformer_zh-cn-common-vocab272727-pytorch/` (Punctuation)