# Tool Result Media Placeholder Fix (#99241) - ⚠️ 已回滚

## 状态：已回滚

**回滚日期：** 2026-07-09

**回滚原因：** 按照 PR #99756 打 patch 后，虽然 `(see attached image)` 占位符不再出现，但 exec 工具在上下文到 ~120K 时返回空对象 `{}`，比不打 patch（可撑到 ~180K）更差。PR #99756 修了表面症状但没解决根因。

**已在 PR #99756 下追加评论说明：** https://github.com/openclaw/openclaw/pull/99756#issuecomment-4922950185

**当前策略：** 不打 patch，等官方 PR #99756 或 #100795 正式合并后再评估。

---

## 原始问题

Issue #99241 - 会话上下文累积后（~150-200k tokens），工具结果被替换为 `(see attached image)` 占位符，模型无法读取工具输出。渐进式恶化：多行输出先变截图，然后所有工具输出全部变截图。

## 根因

不是 webchat UI 渲染 bug，而是 model-visible payload 序列化层缺陷。工具结果在 replay/compaction/projection 过程中 text 被截断或丢失，旁边混入 stale 的 image/media block，导致 fallback 到 `(see attached image)` 占位符，覆盖了原本的文本。

## 原修复方案（已回滚）

核心逻辑：toolResult 的 content 数组里有任何 `type === "text"` 的 block（即使内容被截断为空），就不用 media placeholder。只有纯 media 的 toolResult 才用 placeholder。

## 受影响文件（7个）

| # | 文件 | Provider/Transport |
|---|------|-------------------|
| 1 | `openai-completions-*.js` | OpenAI Completions API（handai/glm-5.2 走这个） |
| 2 | `openai-responses-shared-*.js` | OpenAI Responses API |
| 3 | `openai-transport-stream-*.js` | OpenAI transport 层 |
| 4 | `provider-stream-*.js` | Anthropic transport 层 |
| 5 | `google-shared-*.js` | Google/Gemini |
| 6 | `transport-stream-*.js` | Google transport 层 |
| 7 | `stream-*.js` | Responses replay |

## 回滚操作

```bash
# 从备份恢复
cp /usr/lib/node_modules/openclaw/dist.pre-99241-patch/<file> /usr/lib/node_modules/openclaw/dist/<file>
```

## 引用

- Issue: https://github.com/openclaw/openclaw/issues/99241
- PR: https://github.com/openclaw/openclaw/pull/99756
- 回滚评论: https://github.com/openclaw/openclaw/pull/99756#issuecomment-4922950185
- memory/2026-07-04-0924.md（根因分析）
- memory/2026-07-08.md（patch 记录）
- memory/2026-07-09.md（回滚记录）
