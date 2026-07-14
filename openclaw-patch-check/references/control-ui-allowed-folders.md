# Control UI Attachment "Outside allowed folders" Fix (#10210240)

## 问题

Control UI 附件显示 "Unavailable - Outside allowed folders"。agent workspace 在 `/root/.openclaw/workspace-h0assistant/`，但 Control UI 无法预览该路径下的文件。

## 根因（两层）

### 第一层：前端正则不匹配 `/root/`

前端 `aI()` 函数正则 `/^(\/Users\/[^/]+|\/home\/[^/]+)(?:\/|$)/` 只匹配 macOS(`/Users/`)和 Linux非root(`/home/`)，不匹配 `/root/`(Linux root用户)。

### 第二层：服务端 localMediaPreviewRoots 不包含非默认 agent workspace

`/control-ui-config.json` 端点通过 `getAgentScopedMediaLocalRoots(config, identity.agentId)` 返回 roots。请求时 `identity.agentId = "main"`，所以只返回默认 agent 的 workspace（`/root/.openclaw/workspace`），不包含 `workspace-h0assistant` 等其他 agent workspace。前端拿到 roots 后检查 MEDIA 路径不在其中，显示 "Outside allowed folders"。

## 修复

### 第一层：前端补丁

**文件：** `control-ui/assets/index-*.js`

正则加 `|\/root`：

```diff
- /^(\/Users\/[^/]+|\/home\/[^/]+)(?:\/|$)/
+ /^(\/Users\/[^/]+|\/home\/[^/]+|\/root)(?:\/|$)/
```

### 第二层：服务端补丁

**文件：** `local-roots-*.js`

修改 `getAgentScopedMediaLocalRoots` 函数，遍历 `cfg.agents.list`，把所有 agent 的 workspace 路径都加入 roots：

```diff
  function getAgentScopedMediaLocalRoots(cfg, agentId) {
      const roots = buildMediaLocalRoots(resolveStateDir(), resolveConfigDir());
      const normalizedAgentId = normalizeOptionalString(agentId);
      if (!normalizedAgentId) return roots;
      const workspaceDir = resolveAgentWorkspaceDir(cfg, normalizedAgentId);
      if (!workspaceDir) return roots;
      const normalizedWorkspaceDir = path.resolve(workspaceDir);
      if (!roots.includes(normalizedWorkspaceDir)) roots.push(normalizedWorkspaceDir);
+     // #10210240 patch: add all agent workspaces so Control UI can preview files for any agent
+     const agentList = cfg?.agents?.list;
+     if (Array.isArray(agentList)) {
+         for (const entry of agentList) {
+             if (!entry || typeof entry !== "object") continue;
+             const id = normalizeOptionalString(entry?.id);
+             if (!id || id === normalizedAgentId) continue;
+             const dir = resolveAgentWorkspaceDir(cfg, id);
+             if (!dir) continue;
+             const nd = path.resolve(dir);
+             if (!roots.includes(nd)) roots.push(nd);
+         }
+     }
      return roots;
  }
```

## 检查方式

### 前端

```bash
grep -q '\/root)(?:\/|$)' "$FILE"
```

### 服务端

```bash
grep -q '#10210240 patch' "$FILE"
```

## 修复步骤

### 1. 前端补丁

```bash
FILE=$(ls /usr/lib/node_modules/openclaw/dist/control-ui/assets/index-*.js | head -1)
cp "$FILE" "${FILE}.pre-root-patch"
perl -i -pe 's/\Q/^(\/Users\/[^\/]+|\/home\/[^\/]+)(?:\/|$)/\E/^(\/Users\/[^\/]+|\/home\/[^\/]+|\/root)(?:\/|$)/g' "$FILE"
```

### 2. 服务端补丁

用 Python 做精确文本替换（sed/perl 在处理特殊字符时不可靠）：

```bash
FILE=$(ls /usr/lib/node_modules/openclaw/dist/local-roots-*.js | head -1)
cp "$FILE" "${FILE}.pre-all-agents-patch"

python3 << 'PYEOF'
fpath = "FILE_PATH_PLACEHOLDER"
with open(fpath, 'r') as f:
    content = f.read()

old = '''function getAgentScopedMediaLocalRoots(cfg, agentId) {
\tconst roots = buildMediaLocalRoots(resolveStateDir(), resolveConfigDir());
\tconst normalizedAgentId = normalizeOptionalString(agentId);
\tif (!normalizedAgentId) return roots;
\tconst workspaceDir = resolveAgentWorkspaceDir(cfg, normalizedAgentId);
\tif (!workspaceDir) return roots;
\tconst normalizedWorkspaceDir = path.resolve(workspaceDir);
\tif (!roots.includes(normalizedWorkspaceDir)) roots.push(normalizedWorkspaceDir);
\treturn roots;
}'''

new = '''function getAgentScopedMediaLocalRoots(cfg, agentId) {
\tconst roots = buildMediaLocalRoots(resolveStateDir(), resolveConfigDir());
\tconst normalizedAgentId = normalizeOptionalString(agentId);
\tif (!normalizedAgentId) return roots;
\tconst workspaceDir = resolveAgentWorkspaceDir(cfg, normalizedAgentId);
\tif (!workspaceDir) return roots;
\tconst normalizedWorkspaceDir = path.resolve(workspaceDir);
\tif (!roots.includes(normalizedWorkspaceDir)) roots.push(normalizedWorkspaceDir);
\t// #10210240 patch: add all agent workspaces so Control UI can preview files for any agent
\tconst agentList = cfg?.agents?.list;
\tif (Array.isArray(agentList)) {
\t\tfor (const entry of agentList) {
\t\t\tif (!entry || typeof entry !== "object") continue;
\t\t\tconst id = normalizeOptionalString(entry?.id);
\t\t\tif (!id || id === normalizedAgentId) continue;
\t\t\tconst dir = resolveAgentWorkspaceDir(cfg, id);
\t\t\tif (!dir) continue;
\t\t\tconst nd = path.resolve(dir);
\t\t\tif (!roots.includes(nd)) roots.push(nd);
\t\t}
\t}
\treturn roots;
}'''

if old in content:
    content = content.replace(old, new)
    with open(fpath, 'w') as f:
        f.write(content)
    print("Patch applied successfully")
else:
    print("Old pattern not found - may already be patched")
PYEOF
```

### 3. 重启 Gateway

```bash
systemctl --user restart openclaw-gateway
```

注意：`gateway restart`（SIGUSR1 热重载）不会重新加载 dist 文件，必须用 `systemctl --user restart` 完全重启进程。

### 4. 验证

```bash
# 检查 config 端点返回的 roots 是否包含所有 agent workspace
curl -s -H "Authorization: Bearer <token>" http://127.0.0.1:18789/control-ui-config.json | jq '.localMediaPreviewRoots'
```

应包含 `/root/.openclaw/workspace-h0assistant` 等路径。

## 引用

- Issue: https://github.com/openclaw/openclaw/issues/10210240
- memory/2026-07-08.md
- memory/2026-07-08-1957.md
