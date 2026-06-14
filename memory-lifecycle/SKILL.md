---
name: memory-lifecycle
description: "Audit and manage static injected MD files (MEMORY.md, TOOLS.md). Hot/warm/cold classification, wiki migration, periodic cleanup."
---

# Memory Lifecycle

Manage the lifecycle of static injected content to prevent context bloat and attention dilution.

## When to use

- User says: "audit my MEMORY.md" / "audit TOOLS.md" / "memory审计" / "上下文清理"
- Periodic cron trigger (every 2 weeks)
- Before adding new entries to MEMORY.md or TOOLS.md

## Concepts

| Term | Definition |
|------|-----------|
| **Hot** | Referenced ≥2 times in last 30 days → keep in MD |
| **Warm** | Referenced 1 time → keep, observe next audit |
| **Cold** | Referenced 0 times → migrate to wiki or delete |
| **Static injection** | Files loaded into every session context (MEMORY.md, TOOLS.md, etc.) |

## Workflow

### Step 1: Scan entries

Run scan script on target files:

```bash
python3 <skill_dir>/scripts/scan_entries.py <workspace_path>
```

Outputs JSON list of entries with heading, status, and keywords.

If no Status metadata exists, run initial tagging:
- Each `## heading` block = one entry
- Add `Created: YYYY-MM-DD | Audited: YYYY-MM-DD | Status: warm` to each

### Step 2: Classify usage

For each entry, search session transcripts:

```
memory_search(query="<entry keywords>", corpus="sessions", maxResults=5)
```

Classification:
- hits ≥ 2 → **hot**
- hits = 1 → **warm**
- hits = 0 → **cold**

### Step 3: Migrate cold data

For each cold entry:

1. Check if content already exists in target wiki
2. If not, create wiki entry following schema rules:
   - HZERO-related → `HZERO-wiki`
   - Non-HZERO → `MY-wiki`
3. Replace MD content with one-line reference:
   ```markdown
   详细内容已入库 HZERO-wiki：`wiki/XXX.md`
   ```
4. Use `obsidian` CLI for wiki writes (not raw `write`)

### Step 4: Update metadata

Update `Audited` and `Status` fields for all entries.

### Step 5: Generate report

Write audit report to `.memory-audit/YYYY-MM-DD.md`:

```markdown
# Audit Report - YYYY-MM-DD

## Summary
- Total entries: X | Hot: X | Warm: X | Cold: X

## Details
| Entry | Status | Action |
|-------|--------|--------|
| ... | ... | ... |
```

## Cron setup

Schedule via OpenClaw cron:

```
schedule: { kind: "cron", expr: "0 10 1,15 * *", tz: "Asia/Shanghai" }
delivery: { mode: "announce", channel: "<user's channel>" }
```

## Rules

- Never modify SOUL.md, USER.md, AGENTS.md (red-line files)
- Only audit MEMORY.md and TOOLS.md
- Cold data with long-term value → wiki; outdated → delete
- Use obsidian CLI for wiki writes (not raw `write`)
- Update `Audited` date on every audit, even if status unchanged
