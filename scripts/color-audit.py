"""Extract authored styles and their color usage without reading build/runtime files.

Usage: python color-audit.py PROJECT_ROOT OUTPUT_DIRECTORY
The extraction is an audit artifact, not a stylesheet to load into an application.
"""
import collections
import json
from pathlib import Path
import re
import subprocess
import sys

root, output = (Path(arg).resolve() for arg in sys.argv[1:3])
args = ['rg', '--files', '--hidden', '--no-ignore']
for folder in ('bin', 'obj', '.git', 'node_modules', '.dev', '.cursor', '.agents',
               '.gapcode', '.tmp', 'artifacts', 'packages'):
    args += ['-g', f'!**/{folder}/**']
for extension in ('css', 'razor', 'cshtml', 'html'):
    args += ['-g', f'*.{extension}']
paths = subprocess.check_output(args, cwd=root, text=True).splitlines()
color = re.compile(r'#[\da-f]{3,8}\b|(?:rgba?|hsla?|oklch|color-mix)\([^;{}]+|var\(--(?:sb|sufi|persian)[\w-]+|\b(?:white|black|transparent|currentColor)\b', re.I)
records, extracted = [], []
for name in sorted(paths):
    path = root / name
    source = path.read_text(encoding='utf-8-sig')
    blocks = [(1, source)] if path.suffix == '.css' else [
        (source.count('\n', 0, m.start()) + 1, m[1])
        for m in re.finditer(r'<style\b[^>]*>([\s\S]*?)</style>', source, re.I)]
    if not blocks:
        continue
    relative = path.relative_to(root).as_posix()
    category = 'vendor' if '/vendor/' in relative or '/lib/' in relative else 'generated' if '/_sufi/editors/' in relative or relative.endswith('sufiblazor-editors.css') else 'authored'
    expressions = [css.strip() for _, css in blocks
                   if re.fullmatch(r'@\(\(MarkupString\)[\w.]+\)', css.strip())]
    if expressions and len(expressions) == len(blocks):
        category = 'runtime'
    usages = []
    for line, css in blocks:
        extracted.append(f'/* SOURCE: {relative}:{line} ({category}) */\n{css}\n')
        for offset, value in enumerate(css.splitlines()):
            if color.search(value):
                usages.append({'line': line + offset, 'declaration': value.strip()})
    records.append({'path': relative, 'category': category,
                    'kind': 'css' if path.suffix == '.css' else 'style',
                    'blocks': len(blocks), 'runtime_expressions': expressions, 'usages': usages})
output.mkdir(parents=True, exist_ok=True)
(output / 'styles.css.txt').write_text('\n'.join(extracted), encoding='utf-8')
(output / 'color-usage.json').write_text(json.dumps(records, indent=2), encoding='utf-8')
index = collections.defaultdict(list)
for record in records:
    for usage in record['usages']:
        tokens = re.findall(r'var\(\s*(--[\w-]+)', usage['declaration'])
        literals = re.findall(r'#[\da-f]{3,8}\b|(?:rgba?|hsla?)\([^)]*\)', usage['declaration'], re.I)
        for value in sorted(set(tokens + literals)):
            index[value].append({'path': record['path'], **usage})
(output / 'color-index.json').write_text(json.dumps(dict(sorted(index.items())), indent=2), encoding='utf-8')
print(json.dumps({'files': len(records), 'style_blocks': sum(r['blocks'] for r in records if r['kind'] == 'style'),
                  'categories': dict(collections.Counter(r['category'] for r in records)),
                  'color_declarations': sum(len(r['usages']) for r in records),
                  'output': str(output)}, indent=2))
