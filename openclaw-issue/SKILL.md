---
name: openclaw-issue
description: "Submit bug reports or feature requests to the OpenClaw GitHub repository."
---

# OpenClaw Issue Reporter

Submit issues to `openclaw/openclaw` on GitHub via API.

## Prerequisites

- GitHub token in `~/.bashrc` (line 24, `GITHUB_TOKEN`), user: `syfvb`, scope: `repo`
- `exec` defaults to `/bin/sh` which doesn't load `.bashrc` — always use `bash -c '. ~/.bashrc && ...'`

## Workflow

1. Confirm issue type: **bug** or **feature request**
2. Gather details: title, description, steps to reproduce (bug), expected behavior
3. Check for duplicates: `bash -c '. ~/.bashrc && curl -s -H "Authorization: token $GITHUB_TOKEN" "https://api.github.com/search/issues?q=repo:openclaw/openclaw+is:open+TITLE_KEYWORD" | head -20'`
4. Submit via script: `bash ~/.openclaw/skills/openclaw-issue/scripts/submit.sh "TITLE" "BODY" "bug|enhancement"`
5. Share the created issue URL with the user

## Labels

- `bug` — something isn't working
- `enhancement` — feature request
- `documentation` — docs improvement

## Body Template

```markdown
## Description

[clear description]

## Steps to Reproduce

1. ...
2. ...

## Expected Behavior

[what should happen]

## Actual Behavior

[what happens instead]

## Environment

- OpenClaw version: [version]
- OS: [os]
- Node.js: [version]
```

## Safety

- Always confirm with user before submitting
- Never include tokens, passwords, or private config in issue body
- Search for duplicates first
