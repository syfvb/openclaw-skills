# attachment-normalize 栈溢出修复

## 问题

大 base64 文件触发正则栈溢出（`RangeError: Maximum call stack size exceeded`）。Issue #90098。

## 文件

`/usr/lib/node_modules/openclaw/dist/attachment-normalize-*.js`

## 两处修改

### 修改 1：第 89 行 -- `data:` URL 提取正则

**原始代码：**
```js
const match = /^data:([^;]+);base64,(.*)$/.exec(base64);
```

**修复为：**
```js
const headerEnd = base64.indexOf(';base64,');
const match = headerEnd !== -1 ? [
    base64,
    base64.slice(5, headerEnd),
    base64.slice(headerEnd + 8)
] : null;
```

**原因：** `(.*)$` 配合大字符串触发 V8 正则引擎栈溢出。

### 修改 2：第 59 行 -- `isValidBase64` 验证正则

**原始代码：**
```js
function isValidBase64(value) {
    if (value.length === 0 || value.length % 4 !== 0) return false;
    return /^[A-Za-z0-9+/]+={0,2}$/.test(value);
}
```

**修复为：**
```js
function isValidBase64(value) {
    if (value.length === 0 || value.length % 4 !== 0) return false;
    for (let i = 0; i < value.length; i++) {
        const c = value.charCodeAt(i);
        if (!((c >= 65 && c <= 90) || (c >= 97 && c <= 122) || (c >= 48 && c <= 57) || c === 43 || c === 47 || c === 61)) return false;
    }
    return true;
}
```

**原因：** `+` 量词对大字符串执行 `.test()` 同样触发 V8 正则引擎栈溢出。改为逐字符 `charCodeAt` 遍历，O(n) 无递归无栈消耗。

**发现历史：** 修改 1 于 2026-06-25 首次修复。修改 2 于 2026-07-10 发现并修复（14MB 文件触发，之前测试 6MB 未暴露）。

## 检查方式

```bash
grep -q 'indexOf' "$FILE" && grep -q 'slice' "$FILE" && grep -q 'charCodeAt' "$FILE"
```

- 全部命中 = 两处均已修复
- 缺 `charCodeAt` = 修改 2 缺失，`isValidBase64` 仍会栈溢出
- 缺 `indexOf`/`slice` = 修改 1 缺失，`data:` URL 提取仍会栈溢出

## 修复步骤

1. 备份原文件：
   ```bash
   cp "$FILE" "${FILE}.bak.$(date +%Y%m%d%H%M%S)"
   ```

2. 按上述两处修改逐一替换

3. 改完需重启 Gateway（让用户执行，不要自行重启）

## 引用

- Issue: https://github.com/openclaw/openclaw/issues/90098
- PR: https://github.com/openclaw/openclaw/pull/92223（官方修复，未合并）
- MEMORY.md -> 自定义补丁维护 -> attachment-normalize 栈溢出修复
