---
name: openclaw-patch-check
description: "Check and re-apply local OpenClaw dist patches after upgrade. Run after every openclaw update."
---

# OpenClaw Patch Check

After `openclaw update`, run `scripts/patch-check.sh` to check all registered patches.

## Workflow

1. Run `bash ~/.openclaw/skills/openclaw-patch-check/scripts/patch-check.sh`
2. Script exits 0 if all patches OK, exits 1 if any need re-applying
3. For each patch that needs applying, read its `references/<patch>.md` for fix steps
4. After fixing, re-run the script to confirm
5. If any patch was applied, restart Gateway (ask user first)

## Registering a new patch

Add a patch definition block to `scripts/patches.json`. Each patch has:

```json
{
  "id": "unique-id",
  "name": "Human readable name",
  "file_glob": "glob pattern relative to /usr/lib/node_modules/openclaw/dist/",
  "check_cmd": "shell command using $FILE; exit 0 = already fixed, exit 1 = needs patch",
  "applied_desc": "short note when patch is already applied",
  "needs_desc": "short note when patch needs to be applied",
  "reference": "references/<patch>.md"
}
```

## Current patches

| ID | Issue | File | Type |
|----|-------|------|------|
| attachment-normalize | #90098 regex stack overflow on large base64 (two fixes) | `attachment-normalize-*.js` | backend |
| reply-session-conflict | #98416 WebChat reply session conflicted | `get-reply-*.js` | backend (验证官方修复) |
| control-ui-allowed-folders-frontend | #10210240 Control UI 附件路径白名单-前端 | `control-ui/assets/index-*.js` | frontend |
| control-ui-allowed-folders-backend | #10210240 Control UI 附件路径白名单-服务端 | `local-roots-*.js` | backend |

### 已官方修复，无需本地补丁

| ID | Issue | 官方修复版本 | 说明 |
|----|-------|-------------|------|
| ~~reply-session-source-fix~~ | #98835 backport: revision 收窄 | v2026.7.1-beta.2 | PR #98835 已包含，mergeConcurrentReplySessionMetadata 已在 dist 中 |
| ~~workspace-file-viewer~~ | #100615 workspace file preview | v2026.7.1-beta.2 | 官方代码已包含 statWorkspacePath 逻辑，前端 sessionKind 限制已移除 |

### 已回滚的 patch

| ID | Issue | 原因 | 状态 |
|----|-------|------|------|
| tool-result-placeholder | #99241 tool outputs render as `(see attached image)` | PR #99756 patch 打后上下文到 120K exec 返回空对象，比不打 patch（撑到 180K）更差 | ❌ 已回滚，等官方 PR 合并 |

## Files

- `scripts/patch-check.sh` - main check script
- `scripts/patches.json` - patch registry (extensible)
- `references/attachment-normalize.md` - fix steps for attachment-normalize
- `references/reply-session-conflict.md` - fix steps for reply-session-conflict
- `references/control-ui-allowed-folders.md` - fix steps for control-ui-allowed-folders
- `references/tool-result-media-placeholder.md` - 回滚记录，仅供参考
