# Reply Session Initialization Conflicted

## 状态：✅ 官方已修复（v2026.7.1-beta.2）

不再需要本地补丁。升级到 v2026.7.1+ 后已从源头解决。

## 问题

Issue #98416 - v2026.6.11 后 WebChat 连续发消息报 `reply session initialization conflicted`，会话不可用。

## 根因

v2026.6.11 dist 包含了 `reentrant: true` 调用，但 store-writer-queue 模块是修复前旧版，重入保护被静默丢弃。同时 guard 对比整个 session entry，并发 metadata drift 被误报为冲突。

## 官方三层修复

1. **源头修复**（PR #98835 -> commit `826c84ea`）：收窄 guard 对比范围到 `sessionId`/`sessionFile` 身份字段，允许并发 metadata drift
2. **重入保护**（commit `d2da8c79`）：dist 包含 `isActiveStoreWriter`/`runActiveStoreWriter`，`reentrant: true` 正确处理
3. **重试退避**（缓解）：`retryDelays = [1000, 3000, 5000, 8000]`，`maxRetries = 4`

## 修复文件

- `/usr/lib/node_modules/openclaw/dist/get-reply-C8FxMVwx.js`（v2026.7.1-beta.2）
- `/usr/lib/node_modules/openclaw/dist/store-writer-queue-xTwWMyaG.js`
- `/usr/lib/node_modules/openclaw/dist/session-accessor-BTWXMGZx.js`

## 检查方式

```bash
grep -q 'retryDelays' "$FILE" && grep -q 'maxRetries' "$FILE"
```

- 命中 = 官方修复已到位
- 未命中 = 需排查

## 引用

- Issue: https://github.com/openclaw/openclaw/issues/98416
- PR: https://github.com/openclaw/openclaw/pull/98835
- MEMORY.md -> 自定义补丁维护 -> Reply Session Initialization Conflicted
