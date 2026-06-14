#!/bin/bash
# Submit an issue to openclaw/openclaw on GitHub
# Usage: submit.sh "TITLE" "BODY" "label"
# label: bug | enhancement | documentation

set -e

TITLE="${1:?Usage: submit.sh \"TITLE\" \"BODY\" \"label\"}"
BODY="${2:?Usage: submit.sh \"TITLE\" \"BODY\" \"label\"}"
LABEL="${3:-bug}"

# Load GitHub token
source ~/.bashrc

if [ -z "$GITHUB_TOKEN" ]; then
  echo "ERROR: GITHUB_TOKEN not set in ~/.bashrc"
  exit 1
fi

# Create issue
RESULT=$(curl -s -w "\n%{http_code}" \
  -H "Authorization: token $GITHUB_TOKEN" \
  -H "Accept: application/vnd.github.v3+json" \
  https://api.github.com/repos/openclaw/openclaw/issues \
  -d "$(jq -n \
    --arg title "$TITLE" \
    --arg body "$BODY" \
    --arg lbl "$LABEL" \
    '{title: $title, body: $body, labels: [$lbl]}')")

HTTP_CODE=$(echo "$RESULT" | tail -1)
RESPONSE=$(echo "$RESULT" | sed '$d')

if [ "$HTTP_CODE" = "201" ]; then
  URL=$(echo "$RESPONSE" | jq -r '.html_url')
  NUMBER=$(echo "$RESPONSE" | jq -r '.number')
  echo "OK: #$NUMBER $URL"
else
  echo "FAIL ($HTTP_CODE):"
  echo "$RESPONSE" | jq -r '.message // "unknown error"'
  exit 1
fi
