#!/usr/bin/env python3
"""
Scan MEMORY.md and TOOLS.md for entries with Status metadata.
Output JSON list of entries for downstream audit.
"""
import re
import sys
import json
from pathlib import Path

def scan_entries(filepath):
    """Extract ## heading blocks with Status metadata."""
    text = Path(filepath).read_text()
    entries = []
    
    # Split by ## headings
    blocks = re.split(r'\n(?=## )', text)
    
    for block in blocks:
        if not block.startswith('## '):
            continue
        
        # Extract heading
        heading_match = re.match(r'## (.+)', block)
        if not heading_match:
            continue
        heading = heading_match.group(1).strip()
        
        # Skip meta/transition sections
        skip_headings = ['已迁移冷数据（2026-06-13）', 'Summary', 'Core Content', 'Related Concepts']
        if any(s in heading for s in skip_headings):
            continue
        
        # Extract metadata
        created = re.search(r'\*\*Created:\*\*\s*(\d{4}-\d{2}-\d{2})', block)
        audited = re.search(r'\*\*Audited:\*\*\s*(\d{4}-\d{2}-\d{2})', block)
        status = re.search(r'\*\*Status:\*\*\s*(\w+)', block)
        
        # Extract keywords for search (body text after metadata)
        body_lines = block.split('\n')
        body_text = ' '.join(l for l in body_lines if not l.startswith('**') and not l.startswith('#'))
        keywords = re.sub(r'[|*\-\[\]`]', ' ', body_text)[:150].strip()
        
        entries.append({
            'heading': heading,
            'created': created.group(1) if created else None,
            'audited': audited.group(1) if audited else None,
            'status': status.group(1) if status else 'untagged',
            'keywords': keywords,
            'source_file': str(filepath)
        })
    
    return entries

def main():
    workspace = Path(sys.argv[1]) if len(sys.argv) > 1 else Path('.')
    
    all_entries = []
    for md_file in ['MEMORY.md', 'TOOLS.md']:
        fpath = workspace / md_file
        if fpath.exists():
            entries = scan_entries(fpath)
            all_entries.extend(entries)
    
    print(json.dumps(all_entries, ensure_ascii=False, indent=2))

if __name__ == '__main__':
    main()
