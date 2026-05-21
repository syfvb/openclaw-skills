#!/usr/bin/env python3
"""
视频内容分析脚本 - 抽取关键帧并转写音频，为综合总结提供数据

Usage:
    python3 video_summarize.py <video_file> [--frames N] [--output-dir DIR]
    
Examples:
    python3 video_summarize.py video.mp4
    python3 video_summarize.py video.mp4 --frames 10 --output-dir ./analysis
"""

import os
import sys
import json
import subprocess
import argparse
import shutil

def get_video_info(video_path):
    """获取视频基本信息"""
    try:
        # 获取时长
        result = subprocess.run(
            ['ffprobe', '-v', 'quiet', '-show_entries', 
             'format=duration:stream=width,height,codec_name',
             '-of', 'json', video_path],
            capture_output=True, text=True, timeout=30
        )
        if result.returncode == 0:
            data = json.loads(result.stdout)
            duration = float(data.get('format', {}).get('duration', 0))
            
            # 获取视频流信息
            streams = data.get('streams', [])
            video_stream = None
            for s in streams:
                if s.get('codec_name') not in ['aac', 'mp3', 'opus']:  # 排除音频流
                    video_stream = s
                    break
            
            return {
                'duration': duration,
                'width': video_stream.get('width') if video_stream else None,
                'height': video_stream.get('height') if video_stream else None,
                'codec': video_stream.get('codec_name') if video_stream else None
            }
    except Exception as e:
        print(f"获取视频信息失败: {e}")
    return None


def calculate_frame_interval(duration, max_frames=20):
    """根据视频时长计算关键帧抽取间隔"""
    if duration <= 30:
        # 短视频：每2秒一帧
        interval = 2
    elif duration <= 120:
        # 中等视频：每5秒一帧
        interval = 5
    else:
        # 长视频：每10秒一帧
        interval = 10
    
    # 确保不超过最大帧数
    estimated_frames = int(duration / interval) + 1
    if estimated_frames > max_frames:
        interval = duration / max_frames
    
    return interval


def extract_frames(video_path, output_dir, interval=5, max_frames=20):
    """抽取视频关键帧"""
    frames_dir = os.path.join(output_dir, 'frames')
    os.makedirs(frames_dir, exist_ok=True)
    
    # 使用 ffmpeg 抽取帧
    fps = 1 / interval  # 每秒抽取的帧数
    
    try:
        subprocess.run(
            ['ffmpeg', '-i', video_path,
             '-vf', f'fps={fps},scale=1280:-1',
             '-q:v', '2',
             '-vframes', str(max_frames),
             os.path.join(frames_dir, 'frame_%03d.jpg'),
             '-y'],
            capture_output=True, timeout=120
        )
        
        # 收集抽取的帧文件
        frames = []
        for f in sorted(os.listdir(frames_dir)):
            if f.endswith('.jpg'):
                frames.append(os.path.join(frames_dir, f))
        
        return frames
    except Exception as e:
        print(f"关键帧抽取失败: {e}")
        return []


def extract_audio(video_path, output_dir):
    """从视频提取音频"""
    audio_path = os.path.join(output_dir, 'audio.mp3')
    
    try:
        subprocess.run(
            ['ffmpeg', '-i', video_path,
             '-vn', '-acodec', 'libmp3lame', '-q:a', '2',
             audio_path, '-y'],
            capture_output=True, timeout=60
        )
        
        if os.path.exists(audio_path):
            return audio_path
    except Exception as e:
        print(f"音频提取失败: {e}")
    return None


def transcribe_audio(audio_path):
    """语音转写 - 优先使用 FunASR (本地)，失败后使用 audio-transcription (云端)"""
    # 1. 优先尝试 FunASR skill (本地离线转录)
    funasr_script = os.path.expanduser('~/.openclaw/skills/funasr/scripts/funasr_transcribe.py')
    
    if os.path.exists(funasr_script):
        print("使用 FunASR 进行本地语音转录...")
        try:
            result = subprocess.run(
                ['python3', funasr_script, audio_path],
                capture_output=True, text=True, timeout=600  # FunASR 可能需要更长时间
            )
            
            if result.returncode == 0:
                # FunASR 会输出 JSON 文件到 audio_path 的同名位置
                # 例如 audio.mp3 -> audio_funasr.json
                audio_dir = os.path.dirname(audio_path)
                audio_basename = os.path.splitext(os.path.basename(audio_path))[0]
                json_path = os.path.join(audio_dir, f"{audio_basename}_funasr.json")
                
                # 直接读取 JSON 文件（更可靠的方式）
                if os.path.exists(json_path):
                    with open(json_path, 'r', encoding='utf-8') as f:
                        funasr_result = json.load(f)
                    
                    text = funasr_result.get('text', '')
                    if text and len(text) > 50:  # 确保有效内容
                        # 显示性能信息
                        perf = funasr_result.get('performance', {})
                        speed = perf.get('speed_factor', 0)
                        rtf = perf.get('rtf', 0)
                        print(f"FunASR 转录成功: {len(text)} 字符 (速度: {speed:.1f}x 实时, RTF={rtf:.3f})")
                        return text
                    else:
                        print("FunASR JSON 文件内容无效，尝试备用方案...")
                else:
                    print(f"FunASR JSON 文件不存在: {json_path}，尝试备用方案...")
            else:
                print(f"FunASR 执行失败 (返回码: {result.returncode})，尝试备用方案...")
        except subprocess.TimeoutExpired:
            print("FunASR 处理超时，尝试备用方案...")
        except Exception as e:
            print(f"FunASR 转录出错: {e}，尝试备用方案...")
    else:
        print("FunASR skill 未安装，使用备用方案...")
    
    # 2. 备用方案：使用 audio-transcription skill (云端 DashScope API)
    asr_script = os.path.expanduser('~/.openclaw/skills/audio-transcription/scripts/asr_transcribe.py')
    
    if not os.path.exists(asr_script):
        print("audio-transcription skill 未安装，跳过语音转写")
        return None
    
    print("使用 DashScope API 进行云端语音转录...")
    try:
        result = subprocess.run(
            ['python3', asr_script, audio_path],
            capture_output=True, text=True, timeout=300
        )
        
        # 解析输出
        output = result.stdout
        
        # 提取转写文本（从 "=== 转写结果 ===" 后获取）
        if '=== 转写结果 ===' in output or '转写结果' in output:
            lines = output.split('\n')
            text_start = False
            text_lines = []
            for line in lines:
                if '转写结果' in line:
                    text_start = True
                    continue
                if text_start:
                    if '===' in line:  # 到下一个分隔符
                        break
                    text_lines.append(line.strip())
            
            text = '\n'.join(text_lines).strip()
            if text:
                print(f"DashScope 转录成功: {len(text)} 字符")
            return text
        
        return None
    except subprocess.TimeoutExpired:
        print("DashScope API 处理超时")
        return None
    except Exception as e:
        print(f"语音转写失败: {e}")
        return None


def analyze_video(video_path, output_dir=None, max_frames=20):
    """
    分析视频 - 抽取关键帧并转写音频
    
    Args:
        video_path: 视频文件路径
        output_dir: 输出目录（默认为视频文件同目录下的 analysis 子目录）
        max_frames: 最大抽取帧数
    
    Returns:
        分析结果字典，包含关键帧路径和转写文本
    """
    # 设置输出目录
    if output_dir is None:
        base_name = os.path.splitext(video_path)[0]
        output_dir = f"{base_name}_analysis"
    
    os.makedirs(output_dir, exist_ok=True)
    
    # 获取视频信息
    print(f"分析视频: {video_path}")
    video_info = get_video_info(video_path)
    
    if video_info:
        duration = video_info['duration']
        print(f"时长: {duration:.1f}s, 分辨率: {video_info.get('width')}x{video_info.get('height')}")
    
    # 计算帧抽取间隔
    duration = video_info.get('duration', 60) if video_info else 60
    interval = calculate_frame_interval(duration, max_frames)
    print(f"关键帧抽取间隔: {interval}s")
    
    # 抽取关键帧
    print("抽取关键帧...")
    frames = extract_frames(video_path, output_dir, interval, max_frames)
    print(f"已抽取 {len(frames)} 个关键帧")
    
    # 提取音频
    print("提取音频...")
    audio_path = extract_audio(video_path, output_dir)
    
    # 语音转写
    transcript = None
    if audio_path:
        print("语音转写...")
        transcript = transcribe_audio(audio_path)
        if transcript:
            print(f"转写文本长度: {len(transcript)} 字符")
    
    # 保存结果
    result = {
        'video_path': video_path,
        'video_info': video_info,
        'frames': frames,
        'frame_interval': interval,
        'audio_path': audio_path,
        'transcript': transcript,
        'output_dir': output_dir
    }
    
    result_file = os.path.join(output_dir, 'analysis_result.json')
    with open(result_file, 'w', encoding='utf-8') as f:
        json.dump(result, f, ensure_ascii=False, indent=2)
    
    print(f"\n分析结果已保存到: {result_file}")
    
    return result


def main():
    parser = argparse.ArgumentParser(
        description='视频内容分析 - 抽取关键帧并转写音频'
    )
    parser.add_argument('video_path', help='视频文件路径')
    parser.add_argument('--frames', type=int, default=20,
                        help='最大抽取帧数（默认20）')
    parser.add_argument('--output-dir', '-o', 
                        help='输出目录（默认为视频同目录下的 analysis 子目录）')
    
    args = parser.parse_args()
    
    # 执行分析
    result = analyze_video(args.video_path, args.output_dir, args.frames)
    
    # 打印摘要
    print("\n" + "="*50)
    print("视频分析完成")
    print("="*50)
    
    if result['frames']:
        print(f"\n关键帧文件（供图像分析）:")
        for f in result['frames'][:5]:
            print(f"  {f}")
        if len(result['frames']) > 5:
            print(f"  ... 共 {len(result['frames'])} 个")
    
    if result['transcript']:
        print(f"\n转写文本:")
        print(f"  {result['transcript'][:200]}...")
    
    print(f"\n下一步: 使用 image 工具分析关键帧，综合生成视频总结")


if __name__ == '__main__':
    main()