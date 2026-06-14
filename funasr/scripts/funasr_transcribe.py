#!/usr/bin/env python3
"""
FunASR Local ASR Transcription Script
离线语音识别脚本 - 使用 FunASR Paraformer 模型

Usage:
    python3 funasr_transcribe.py <audio_or_video_file> [-o output.json] [--model offline|streaming] [--verbose]
"""

import argparse
import json
import os
import subprocess
import sys
import time
from pathlib import Path

# 检查依赖
def check_dependencies():
    """检查必要的依赖是否已安装"""
    missing = []
    
    try:
        import funasr
    except ImportError:
        missing.append("funasr")
    
    try:
        import torch
    except ImportError:
        missing.append("torch")
    
    # 检查 ffmpeg
    try:
        subprocess.run(["ffmpeg", "-version"], capture_output=True, check=True)
    except:
        missing.append("ffmpeg")
    
    if missing:
        print("❌ 缺少依赖:")
        for dep in missing:
            if dep in ["funasr", "torch"]:
                print(f"   - {dep}: pip install {dep} modelscope")
            else:
                print(f"   - {dep}: apt install {dep} 或 brew install {dep}")
        sys.exit(1)
    
    return True


def get_audio_duration(audio_path):
    """获取音频时长"""
    try:
        result = subprocess.run(
            ["ffprobe", "-v", "error", "-show_entries", "format=duration",
             "-of", "default=noprint_wrappers=1:nokey=1", audio_path],
            capture_output=True, text=True
        )
        return float(result.stdout.strip())
    except:
        return 0


def extract_audio(video_path, output_audio=None):
    """从视频中提取音频"""
    if output_audio is None:
        output_audio = video_path.replace(os.path.splitext(video_path)[1], "_audio.wav")
    
    print(f"提取音频: {video_path} -> {output_audio}")
    
    # 提取为 WAV 16kHz 单声道（FunASR 最佳输入格式）
    subprocess.run([
        "ffmpeg", "-y", "-i", video_path,
        "-vn", "-acodec", "pcm_s16le",
        "-ar", "16000", "-ac", "1",
        output_audio
    ], capture_output=True, check=True)
    
    return output_audio


def transcribe_with_funasr(audio_path, model_type="offline", verbose=False):
    """使用 FunASR 进行语音识别"""
    from funasr import AutoModel
    
    # 选择模型
    if model_type == "streaming":
        model_name = "paraformer-zh-streaming"
        vad_model = None
        punc_model = None
    else:
        model_name = "paraformer-zh"
        vad_model = "fsmn-vad"
        punc_model = "ct-punc-c"
    
    print(f"\n{'='*60}")
    print(f"FunASR 本地语音识别")
    print(f"{'='*60}")
    print(f"音频文件: {audio_path}")
    print(f"模型: {model_name}")
    print(f"模式: {model_type}")
    
    # 获取音频时长
    duration = get_audio_duration(audio_path)
    print(f"时长: {duration:.1f}s ({duration/60:.1f}分钟)")
    
    # 加载模型
    print("\n加载模型...")
    start_load = time.time()
    
    model = AutoModel(
        model=model_name,
        model_revision="v2.0.4",
        vad_model=vad_model,
        vad_model_revision="v2.0.4" if vad_model else None,
        punc_model=punc_model,
        punc_model_revision="v2.0.4" if punc_model else None,
        disable_update=True,
        disable_log=True,
    )
    
    load_time = time.time() - start_load
    print(f"模型加载耗时: {load_time:.1f}s")
    
    # 执行识别
    print("\n开始识别...")
    start_transcribe = time.time()
    
    result = model.generate(input=audio_path)
    
    transcribe_time = time.time() - start_transcribe
    print(f"识别耗时: {transcribe_time:.1f}s")
    
    # 计算性能指标
    rtf = transcribe_time / duration if duration > 0 else 0
    speed_factor = duration / transcribe_time if transcribe_time > 0 else 0
    
    print(f"\n性能: {speed_factor:.1f}x 实时 (RTF={rtf:.3f})")
    
    # 解析结果
    if result and len(result) > 0:
        text = result[0].get("text", "")
        
        # 解析句子级时间戳（如果有）
        sentences = []
        if "sentences" in result[0]:
            for sent in result[0]["sentences"]:
                sentences.append({
                    "begin_time": sent.get("start", 0),
                    "end_time": sent.get("end", 0),
                    "text": sent.get("text", "")
                })
        
        output = {
            "text": text,
            "sentences": sentences,
            "model": model_name,
            "model_type": model_type,
            "audio_info": {
                "path": audio_path,
                "duration": duration,
                "format": os.path.splitext(audio_path)[1],
            },
            "performance": {
                "load_time_sec": load_time,
                "transcribe_time_sec": transcribe_time,
                "total_time_sec": load_time + transcribe_time,
                "rtf": rtf,
                "speed_factor": speed_factor,
            }
        }
        
        return output
    else:
        return {"error": "No transcription result", "text": ""}


def main():
    parser = argparse.ArgumentParser(
        description="FunASR 本地语音识别脚本",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
示例:
  # 基本使用
  python3 funasr_transcribe.py video.mp4
  
  # 输出到 JSON
  python3 funasr_transcribe.py audio.mp3 -o result.json
  
  # 使用流式模型（更快）
  python3 funasr_transcribe.py audio.mp3 --model streaming
  
  # 详细输出
  python3 funasr_transcribe.py video.mp4 --verbose
"""
    )
    
    parser.add_argument("input", help="音频或视频文件路径")
    parser.add_argument("-o", "--output", help="输出 JSON 文件路径")
    parser.add_argument("--model", choices=["offline", "streaming"], default="offline",
                        help="模型类型: offline (高精度) 或 streaming (快速)")
    parser.add_argument("--verbose", "-v", action="store_true", help="显示详细输出")
    
    args = parser.parse_args()
    
    # 检查依赖
    check_dependencies()
    
    # 检查输入文件
    input_path = args.input
    if not os.path.exists(input_path):
        print(f"❌ 文件不存在: {input_path}")
        sys.exit(1)
    
    # 判断是否是视频文件
    video_extensions = ['.mp4', '.mov', '.avi', '.mkv', '.webm', '.flv', '.wmv']
    is_video = os.path.splitext(input_path)[1].lower() in video_extensions
    
    # 如果是视频，提取音频
    if is_video:
        audio_path = extract_audio(input_path)
    else:
        audio_path = input_path
    
    # 执行识别
    result = transcribe_with_funasr(audio_path, args.model, args.verbose)
    
    # 显示结果
    if "error" not in result:
        print(f"\n{'='*60}")
        print(f"识别结果 (文本长度: {len(result['text'])} 字符)")
        print(f"{'='*60}")
        
        # 显示前 500 字符
        preview = result["text"][:500]
        print(preview)
        if len(result["text"]) > 500:
            print(f"\n... (还有 {len(result['text'])-500} 字符)")
    
    # 保存结果
    if args.output:
        with open(args.output, "w", encoding="utf-8") as f:
            json.dump(result, f, ensure_ascii=False, indent=2)
        print(f"\n✅ 结果已保存: {args.output}")
    else:
        # 默认输出到同级目录
        output_path = input_path.replace(os.path.splitext(input_path)[1], "_funasr.json")
        with open(output_path, "w", encoding="utf-8") as f:
            json.dump(result, f, ensure_ascii=False, indent=2)
        print(f"\n✅ 结果已保存: {output_path}")
    
    # 清理临时音频文件
    if is_video and os.path.exists(audio_path) and audio_path != input_path:
        if args.verbose:
            print(f"清理临时文件: {audio_path}")
        os.remove(audio_path)
    
    return result


if __name__ == "__main__":
    main()