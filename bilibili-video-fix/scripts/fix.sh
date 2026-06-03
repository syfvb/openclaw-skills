#!/bin/bash
# B站视频兼容性修复脚本
# 用法: bash fix.sh <input.mp4> [output_name]
# 输出: output_name_fixed.mp4 + output_name_fixed.zip

set -e

INPUT="$1"
NAME="${2:-$(basename "$INPUT" .mp4)}"

if [ -z "$INPUT" ]; then
  echo "用法: bash fix.sh <input.mp4> [output_name]"
  echo "示例: bash fix.sh video.mp4 my_video"
  exit 1
fi

if [ ! -f "$INPUT" ]; then
  echo "错误: 文件不存在 - $INPUT"
  exit 1
fi

OUTPUT="${NAME}_fixed.mp4"
ZIP="${NAME}_fixed.zip"

echo "=== B站视频兼容性修复 ==="
echo "输入: $INPUT"
echo ""

# 检查原始编码
echo "[1/3] 检查原始编码..."
CODEC=$(ffprobe -v error -select_streams v:0 -show_entries stream=codec_name -of csv=p=0 "$INPUT" 2>/dev/null)
echo "  原始编码: $CODEC"

if [ "$CODEC" = "h264" ]; then
  echo "  已经是 H.264，但仍会优化参数以确保兼容性"
fi

# 转码
echo ""
echo "[2/3] 转码为 H.264 Baseline..."
ffmpeg -i "$INPUT" \
  -c:v libx264 -profile:v baseline -level 3.1 \
  -pix_fmt yuv420p \
  -colorspace bt709 -color_primaries bt709 -color_trc bt709 \
  -vf "setsar=1:1" \
  -c:a aac -ac 2 -ar 44100 -b:a 96k \
  -movflags +faststart \
  -map_metadata -1 \
  -f mp4 "$OUTPUT" -y 2>/dev/null

echo "  完成: $OUTPUT ($(du -h "$OUTPUT" | cut -f1))"

# 打包 zip
echo ""
echo "[3/3] 打包 zip..."
zip -j "$ZIP" "$OUTPUT" 2>/dev/null
echo "  完成: $ZIP ($(du -h "$ZIP" | cut -f1))"

echo ""
echo "=== 全部完成 ==="
echo "  视频文件: $OUTPUT"
echo "  zip 文件: $ZIP"
echo ""
echo "微信传输建议用 zip 文件，接收方解压即可使用。"
