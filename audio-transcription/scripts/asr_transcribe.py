#!/usr/bin/env python3
"""
通用语音识别脚本 - 使用阿里云 DashScope API
支持任意长度音频，自动选择最优方案

Usage:
    python3 asr_transcribe.py <audio_file_path> [--model paraformer-v2|qwen3-asr-flash]
    
Examples:
    python3 asr_transcribe.py video.mp4
    python3 asr_transcribe.py audio.mp3 --model qwen3-asr-flash
"""

import os
import sys
import json
import time
import argparse
import subprocess
import requests

# DashScope API endpoints
API_URL_SUBMIT = "https://dashscope.aliyuncs.com/api/v1/services/audio/asr/transcription"
API_URL_QUERY_BASE = "https://dashscope.aliyuncs.com/api/v1/tasks/"
API_URL_FILES = "https://dashscope.aliyuncs.com/api/v1/files"

# SDK 方式最大支持时长（秒）
SDK_MAX_DURATION = 300  # 5分钟


def get_api_key():
    """从 OpenClaw 配置文件获取 DashScope API Key"""
    config_path = "/root/.openclaw/openclaw.json"
    if os.path.exists(config_path):
        with open(config_path, 'r') as f:
            config = json.load(f)
            # 尝试 aliyun provider
            api_key = config.get('models', {}).get('providers', {}).get('aliyun', {}).get('apiKey')
            if api_key and not api_key.startswith('__OPEN'):
                return api_key
            # 尝试 dashscope provider
            api_key = config.get('models', {}).get('providers', {}).get('dashscope', {}).get('apiKey')
            if api_key and not api_key.startswith('__OPEN'):
                return api_key
    
    # 尝试环境变量
    api_key = os.environ.get('DASHSCOPE_API_KEY')
    if api_key:
        return api_key
    
    raise Exception("未找到 DashScope API Key，请配置 openclaw.json 或设置 DASHSCOPE_API_KEY 环境变量")


def get_audio_duration(audio_path):
    """获取音频时长（秒）"""
    try:
        result = subprocess.run(
            ['ffprobe', '-v', 'quiet', '-show_entries', 'format=duration', 
             '-of', 'default=noprint_wrappers=1:nokey=1', audio_path],
            capture_output=True, text=True, timeout=30
        )
        if result.returncode == 0:
            return float(result.stdout.strip())
    except:
        pass
    return None


def extract_audio_from_video(video_path, output_path=None):
    """从视频文件提取音频"""
    if output_path is None:
        base_name = os.path.splitext(video_path)[0]
        output_path = f"{base_name}_audio.mp3"
    
    # 检查是否已是音频文件
    audio_extensions = ['.mp3', '.wav', '.flac', '.aac', '.ogg', '.m4a']
    if os.path.splitext(video_path)[1].lower() in audio_extensions:
        return video_path
    
    # 提取音频
    try:
        subprocess.run(
            ['ffmpeg', '-i', video_path, '-vn', '-acodec', 'libmp3lame', 
             '-q:a', '2', output_path, '-y'],
            capture_output=True, timeout=60
        )
        return output_path
    except Exception as e:
        raise Exception(f"音频提取失败: {e}")


def transcribe_sdk(audio_path, api_key):
    """
    SDK 方式 - 使用 qwen3-asr-flash 模型，支持 <=5 分钟音频
    简单快速，无需上传文件
    """
    try:
        import dashscope
        dashscope.base_http_api_url = 'https://dashscope.aliyuncs.com/api/v1'
        
        # 设置 API key (SDK 需要特殊方式设置)
        dashscope.api_key = api_key
        
        # 使用绝对路径格式
        abs_path = os.path.abspath(audio_path)
        audio_file_path = f"file://{abs_path}"
        
        messages = [
            {"role": "user", "content": [{"audio": audio_file_path}]}
        ]
        
        response = dashscope.MultiModalConversation.call(
            model="qwen3-asr-flash",
            messages=messages,
            result_format="message",
            asr_options={"enable_itn": False}
        )
        
        if response.get('status_code') == 200:
            content = response['output']['choices'][0]['message']['content'][0]['text']
            return {
                'text': content,
                'model': 'qwen3-asr-flash',
                'method': 'sdk'
            }
        else:
            raise Exception(f"ASR 失败: {response.get('message')}")
    except ImportError:
        raise Exception("dashscope SDK 未安装，请运行: pip install dashscope")


def upload_file(audio_path, api_key):
    """上传音频文件到 DashScope Files API"""
    headers = {"Authorization": f"Bearer {api_key}"}
    
    with open(audio_path, 'rb') as f:
        audio_data = f.read()
    
    files = {'file': (os.path.basename(audio_path), audio_data, 'audio/mpeg')}
    data = {'purpose': 'asr'}
    
    resp = requests.post(API_URL_FILES, headers=headers, files=files, timeout=60)
    
    if resp.status_code != 200:
        raise Exception(f"文件上传失败: {resp.text}")
    
    result = resp.json()
    uploaded = result.get('data', {}).get('uploaded_files', [])
    if not uploaded:
        raise Exception(f"上传返回异常: {result}")
    
    return uploaded[0]['file_id']


def get_file_url(file_id, api_key):
    """获取上传文件的公网 URL"""
    headers = {"Authorization": f"Bearer {api_key}"}
    
    resp = requests.get(f"{API_URL_FILES}/{file_id}", headers=headers, timeout=30)
    
    if resp.status_code != 200:
        raise Exception(f"获取文件信息失败: {resp.text}")
    
    result = resp.json()
    return result.get('data', {}).get('url')


def transcribe_async(audio_path, api_key, model="paraformer-v2"):
    """
    异步 API 方式 - 使用 paraformer-v2 模型
    支持任意长度音频，需上传文件
    """
    # 1. 上传文件
    print(f"上传文件: {audio_path}")
    file_id = upload_file(audio_path, api_key)
    print(f"文件 ID: {file_id}")
    
    # 2. 获取文件 URL（需刷新，STS token 有时效）
    file_url = get_file_url(file_id, api_key)
    
    # 3. 提交 ASR 任务
    headers = {
        "Authorization": f"Bearer {api_key}",
        "Content-Type": "application/json",
        "X-DashScope-Async": "enable"
    }
    
    payload = {
        "model": model,
        "input": {"file_urls": [file_url]}
    }
    
    resp = requests.post(API_URL_SUBMIT, headers=headers, json=payload, timeout=30)
    
    if resp.status_code != 200:
        raise Exception(f"ASR 任务提交失败: {resp.text}")
    
    task_id = resp.json()['output']['task_id']
    print(f"任务 ID: {task_id}")
    
    # 4. 轮询任务状态
    print("等待转写完成...")
    for i in range(60):  # 最长等待 2 分钟
        time.sleep(2)
        
        query_resp = requests.get(f"{API_URL_QUERY_BASE}{task_id}", headers=headers, timeout=30)
        query_data = query_resp.json()
        status = query_data.get('output', {}).get('task_status', 'UNKNOWN')
        
        if status == 'SUCCEEDED':
            results = query_data.get('output', {}).get('results', [])
            if results:
                result_url = results[0].get('transcription_url')
                if result_url:
                    result_resp = requests.get(result_url, timeout=30)
                    result_data = result_resp.json()
                    transcripts = result_data.get('transcripts', [])
                    if transcripts:
                        text = transcripts[0].get('text', '')
                        sentences = transcripts[0].get('sentences', [])
                        
                        return {
                            'text': text,
                            'sentences': sentences,
                            'model': model,
                            'method': 'async',
                            'audio_info': result_data.get('audio_info', {}),
                            'file_id': file_id,
                            'task_id': task_id
                        }
        elif status == 'FAILED':
            error_msg = query_data.get('output', {}).get('message', 'Unknown error')
            raise Exception(f"ASR 任务失败: {error_msg}")
    
    raise Exception("ASR 任务超时")


def transcribe(audio_path, model=None):
    """
    主函数 - 自动选择最优方案进行语音识别
    
    Args:
        audio_path: 音频/视频文件路径
        model: 指定模型 (qwen3-asr-flash | paraformer-v2)，默认自动选择
    
    Returns:
        转写结果字典
    """
    api_key = get_api_key()
    
    # 确保是音频文件
    audio_file = extract_audio_from_video(audio_path)
    
    # 获取时长
    duration = get_audio_duration(audio_file)
    
    # 自动选择方案
    if model == 'qwen3-asr-flash' or (model is None and duration and duration <= SDK_MAX_DURATION):
        # SDK 方式 - 短音频优先
        try:
            print(f"使用 SDK 方式 (qwen3-asr-flash)，时长: {duration:.1f}s")
            return transcribe_sdk(audio_file, api_key)
        except Exception as e:
            print(f"SDK 方式失败: {e}")
            if model == 'qwen3-asr-flash':
                raise
            # 降级到异步方式
            print("降级到异步 API 方式...")
    
    # 异步 API 方式 - 长音频或 SDK 失败时
    model = model or 'paraformer-v2'
    print(f"使用异步 API 方式 ({model})")
    return transcribe_async(audio_file, api_key, model)


def main():
    parser = argparse.ArgumentParser(
        description='语音识别脚本 - 支持任意长度音频'
    )
    parser.add_argument('audio_path', help='音频/视频文件路径')
    parser.add_argument('--model', choices=['qwen3-asr-flash', 'paraformer-v2'],
                        help='指定模型（默认自动选择）')
    parser.add_argument('--output', '-o', help='输出文件路径（JSON格式）')
    
    args = parser.parse_args()
    
    # 执行转写
    result = transcribe(args.audio_path, args.model)
    
    # 输出结果
    print("\n=== 转写结果 ===")
    print(result['text'])
    
    if result.get('sentences'):
        print("\n=== 时间轴 ===")
        for s in result['sentences']:
            begin_s = s.get('begin_time', 0) / 1000
            end_s = s.get('end_time', 0) / 1000
            print(f"[{begin_s:.1f}s - {end_s:.1f}s] {s['text']}")
    
    # 保存到文件
    if args.output:
        with open(args.output, 'w', encoding='utf-8') as f:
            json.dump(result, f, ensure_ascii=False, indent=2)
        print(f"\n结果已保存到: {args.output}")
    
    return result


if __name__ == '__main__':
    main()