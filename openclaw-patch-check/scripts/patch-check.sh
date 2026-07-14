#!/usr/bin/env bash
# OpenClaw Patch Check Script
# Checks all registered patches against current dist files.
# Exit 0 = all patches OK, Exit 1 = some patches need re-applying
set -euo pipefail

DIST_DIR="/usr/lib/node_modules/openclaw/dist"
PATCHES_FILE="$(dirname "$0")/patches.json"
NEEDS_FIX=0

if [ ! -f "$PATCHES_FILE" ]; then
  echo "ERROR: patches.json not found at $PATCHES_FILE"
  exit 2
fi

PATCH_COUNT=$(jq 'length' "$PATCHES_FILE")

echo "================================"
echo "OpenClaw Patch Check"
echo "================================"
echo "Dist: $DIST_DIR"
echo "Patches registered: $PATCH_COUNT"
echo ""

for i in $(seq 0 $((PATCH_COUNT - 1))); do
  ID=$(jq -r ".[$i].id" "$PATCHES_FILE")
  NAME=$(jq -r ".[$i].name" "$PATCHES_FILE")
  FILE_GLOB=$(jq -r ".[$i].file_glob" "$PATCHES_FILE")
  CHECK_CMD=$(jq -r ".[$i].check_cmd" "$PATCHES_FILE")
  APPLIED_DESC=$(jq -r ".[$i].applied_desc" "$PATCHES_FILE")
  NEEDS_DESC=$(jq -r ".[$i].needs_desc" "$PATCHES_FILE")
  REFERENCE=$(jq -r ".[$i].reference" "$PATCHES_FILE")

  # Resolve file via glob
  FILE=$(ls "$DIST_DIR"/$FILE_GLOB 2>/dev/null | head -1)

  if [ -z "$FILE" ]; then
    echo "⚠️  [$ID] $NAME"
    echo "   File not found: $FILE_GLOB"
    echo "   (可能文件名已变更，需更新 patches.json)"
    echo ""
    NEEDS_FIX=1
    continue
  fi

  # Run check command
  if eval "$CHECK_CMD" 2>/dev/null; then
    echo "✅ [$ID] $NAME"
    echo "   $APPLIED_DESC"
  else
    echo "❌ [$ID] $NAME"
    echo "   $NEEDS_DESC"
    echo "   File: $FILE"
    echo "   Fix steps: $(dirname "$PATCHES_FILE")/../$REFERENCE"
    NEEDS_FIX=1
  fi
  echo ""
done

echo "================================"
if [ "$NEEDS_FIX" -eq 0 ]; then
  echo "✅ All patches OK"
  exit 0
else
  echo "❌ Some patches need re-applying"
  echo "   Read reference docs and apply fixes, then re-run this script."
  exit 1
fi
