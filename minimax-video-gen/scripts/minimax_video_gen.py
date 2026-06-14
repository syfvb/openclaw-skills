#!/usr/bin/env python3
"""
MiniMax Video Generation Script
使用 MiniMax 海螺 API 生成视频（图生视频模式）
"""

import os
import sys
import time
import json
import requests
import oss2
from pathlib import Path


def get_env_or_exit(name):
    """从环境变量获取配置，不存在则退出"""
    value = os.environ.get(name)
    if not value:
        print(f"❌ 环境变量 {name} 未配置")
        sys.exit(1)
    return value


def upload_to_oss(bucket, local_file, oss_key):
    """上传文件到 OSS"""
    bucket.put_object_from_file(oss_key, local_file)
    return True


def generate_signed_url(bucket, oss_key, expires=3600):
    """生成 OSS 签名 URL"""
    return bucket.sign_url('GET', oss_key, expires)


def submit_video_task(api_key, prompt, image_url, model="I2V-01", duration=6, resolution="1080P"):
    """提交视频生成任务"""
    url = "https://api.minimaxi.com/v1/video_generation"
    payload = {
        "prompt": prompt,
        "first_frame_image": image_url,
        "model": model,
        "duration": duration,
        "resolution": resolution
    }
    headers = {
        "Authorization": f"Bearer {api_key}",
        "Content-Type": "application/json"
    }
    response = requests.post(url, headers=headers, json=payload, timeout=30)
    result = response.json()
    
    if "task_id" not in result:
        raise Exception(f"任务提交失败: {json.dumps(result, ensure_ascii=False)}")
    
    return result["task_id"]


def poll_task_status(api_key, task_id, max_wait=600, interval=10):
    """轮询任务状态"""
    url = "https://api.minimaxi.com/v1/query/video_generation"
    headers = {"Authorization": f"Bearer {api_key}"}
    
    for i in range(max_wait // interval):
        time.sleep(interval)
        response = requests.get(url, headers=headers, params={"task_id": task_id}, timeout=30)
        result = response.json()
        status = result.get("status", "Unknown")
        
        print(f"  [{i*interval}s] 状态: {status}")
        
        if status == "Success":
            return result.get("file_id")
        elif status == "Fail":
            raise Exception(f"任务失败: {json.dumps(result, ensure_ascii=False)}")
    
    raise Exception("任务超时")


def get_video_download_url(api_key, file_id):
    """获取视频下载链接"""
    url = "https://api.minimaxi.com/v1/files/retrieve"
    headers = {"Authorization": f"Bearer {api_key}"}
    response = requests.get(url, headers=headers, params={"file_id": file_id}, timeout=30)
    result = response.json()
    
    download_url = result.get("file", {}).get("download_url")
    if not download_url:
        raise Exception("无法获取下载链接")
    
    return download_url


def download_video(download_url, local_path):
    """下载视频到本地"""
    response = requests.get(download_url, timeout=120)
    with open(local_path, "wb") as f:
        f.write(response.content)
    return local_path


def main():
    if len(sys.argv) < 3:
        print("用法: python3 minimax_video_gen.py <图片路径> \"<动作描述>\" [模型] [时长] [分辨率]")
        print("示例: python3 minimax_video_gen.py photo.jpg \"让画面中的猫咪亲一下小女孩的脸颊\"")
        sys.exit(1)
    
    image_path = sys.argv[1]
    prompt = sys.argv[2]
    model = sys.argv[3] if len(sys.argv) > 3 else "I2V-01"
    duration = int(sys.argv[4]) if len(sys.argv) > 4 else 6
    resolution = sys.argv[5] if len(sys.argv) > 5 else "1080P"
    
    # 检查图片是否存在
    if not os.path.exists(image_path):
        print(f"❌ 图片文件不存在: {image_path}")
        sys.exit(1)
    
    # 获取配置
    api_key = get_env_or_exit("MINIMAX_API_KEY")
    oss_endpoint = get_env_or_exit("ALIYUN_OSS_ENDPOINT")
    oss_bucket_name = get_env_or_exit("ALIYUN_OSS_BUCKET")
    oss_access_key_id = get_env_or_exit("ALIYUN_OSS_ACCESS_KEY_ID")
    oss_access_key_secret = get_env_or_exit("ALIYUN_OSS_ACCESS_KEY_SECRET")
    
    # 初始化 OSS
    print("步骤 1: 上传图片到 OSS...")
    auth = oss2.Auth(oss_access_key_id, oss_access_key_secret)
    bucket = oss2.Bucket(auth, f"https://{oss_endpoint}", oss_bucket_name)
    
    image_filename = os.path.basename(image_path)
    oss_key = f"minimax_video_input/{image_filename}"
    upload_to_oss(bucket, image_path, oss_key)
    print("  ✅ 上传成功")
    
    # 生成签名 URL
    print("步骤 2: 生成签名 URL...")
    signed_url = generate_signed_url(bucket, oss_key)
    print(f"  ✅ 签名 URL 生成成功")
    
    # 提交视频生成任务
    print("步骤 3: 调用 MiniMax 视频生成 API...")
    task_id = submit_video_task(api_key, prompt, signed_url, model, duration, resolution)
    print(f"  ✅ task_id: {task_id}")
    
    # 轮询任务状态
    print("步骤 4: 轮询任务状态...")
    file_id = poll_task_status(api_key, task_id)
    print(f"  ✅ file_id: {file_id}")
    
    # 获取视频下载链接
    print("步骤 5: 获取视频下载链接...")
    download_url = get_video_download_url(api_key, file_id)
    print("  ✅ 下载链接获取成功")
    
    # 下载视频到本地
    print("步骤 6: 下载视频到本地...")
    local_video = os.path.join(os.path.dirname(image_path), f"minimax_video_{int(time.time())}.mp4")
    download_video(download_url, local_video)
    print(f"  ✅ 视频已保存到: {local_video}")
    
    # 上传视频到 OSS 并生成签名链接
    print("步骤 7: 上传视频到 OSS...")
    video_oss_key = f"minimax_video_output/{os.path.basename(local_video)}"
    upload_to_oss(bucket, local_video, video_oss_key)
    video_signed_url = generate_signed_url(bucket, video_oss_key)
    print("  ✅ 视频签名链接生成成功")
    
    # 输出结果
    print("\n" + "="*60)
    print("🎉 完成！视频签名链接（1小时有效）:")
    print(video_signed_url)
    print("="*60)
    
    return video_signed_url


if __name__ == "__main__":
    main()
