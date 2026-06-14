#!/bin/bash
# Render Mermaid diagrams to PNG
# Usage: render_mermaid.sh <input.mmd> <output.png> [width]

set -euo pipefail

INPUT="${1:?Missing input .mmd file}"
OUTPUT="${2:?Missing output .png file}"
WIDTH="${3:-1200}"

# Ensure output directory exists
mkdir -p "$(dirname "$OUTPUT")"

# Method 1: mermaid-cli (mmdc)
if command -v mmdc &>/dev/null; then
    mmdc -i "$INPUT" -o "$OUTPUT" -w "$WIDTH" -b white
    echo "Rendered: $INPUT -> $OUTPUT (via mmdc, ${WIDTH}px)"
    exit 0
fi

# Method 2: Use headless browser with mermaid.live API
if command -v chromium-browser &>/dev/null || command -v chromium &>/dev/null || command -v google-chrome &>/dev/null; then
    BROWSER=$(command -v chromium-browser || command -v chromium || command -v google-chrome)
    # Read Mermaid content
    CONTENT=$(cat "$INPUT" | python3 -c "import json,sys; print(json.dumps(sys.stdin.read()))")
    HTML=$(cat <<EOF
<!DOCTYPE html>
<html><head><script src="https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.min.js"></script></head>
<body><pre class="mermaid">${CONTENT}</pre>
<script>mermaid.run({querySelector:'.mermaid'}).then(()=>{document.title='DONE'});</script></body></html>
EOF
    )
    echo "$HTML" > /tmp/mermaid_render.html
    "$BROWSER" --headless --disable-gpu --screenshot="$OUTPUT" --window-size="${WIDTH%,*},1080" \
        --virtual-time-budget=10000 "file:///tmp/mermaid_render.html" 2>/dev/null
    rm -f /tmp/mermaid_render.html
    echo "Rendered: $INPUT -> $OUTPUT (via headless browser, ${WIDTH}px)"
    exit 0
fi

# Method 3: Use puppeteer via node
if command -v node &>/dev/null && npm list -g puppeteer &>/dev/null 2>&1; then
    node -e "
    const puppeteer = require('puppeteer');
    const fs = require('fs');
    (async () => {
        const browser = await puppeteer.launch({headless: true});
        const page = await browser.newPage();
        await page.setViewport({width: $WIDTH, height: 1080});
        const content = fs.readFileSync('$INPUT', 'utf-8');
        await page.setContent('<html><head><script src=\"https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.min.js\"></script></head><body><pre class=\"mermaid\">' + content + '</pre><script>mermaid.run({querySelector:\".mermaid\"})</script></body></html>');
        await page.waitForFunction('document.title === \"DONE\" || document.querySelector(\".mermaid svg\")');
        await page.screenshot({path: '$OUTPUT', fullPage: true});
        await browser.close();
        console.log('Rendered: $INPUT -> $OUTPUT (via puppeteer)');
    })();
    "
    exit 0
fi

# Fallback: Generate placeholder
echo "WARNING: No Mermaid renderer available. Install mmdc: npm install -g @mermaid-js/mermaid-cli"
echo "Creating placeholder: $OUTPUT"
# Create an empty PNG as placeholder (1x1 pixel)
printf '\x89PNG\r\n\x1a\n\x00\x00\x00\rIHDR\x00\x00\x00\x01\x00\x00\x00\x01\x08\x06\x00\x00\x00\x1f\x15\xc4\x89\x00\x00\x00\nIDATx\x9cc\xf8\x0f\x00\x00\x01\x01\x00\x05\x18\xd8N\x00\x00\x00\x00IEND\xaeB`\x82' > "$OUTPUT"
exit 1
