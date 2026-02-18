#!/usr/bin/env bash
set -euo pipefail

usage() {
  echo "Usage: ./scaffold.sh --name <Project.Name> [--output <directory>]"
}

project_name=""
output_dir="$(pwd)"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --name|-n)
      project_name="$2"
      shift 2
      ;;
    --output|-o)
      output_dir="$2"
      shift 2
      ;;
    --help|-h)
      usage
      exit 0
      ;;
    *)
      echo "Unknown option: $1"
      usage
      exit 1
      ;;
  esac
done

if [[ -z "$project_name" ]]; then
  usage
  exit 1
fi

template_name="BoricuaCoder.CleanTemplate"
template_db_name="cleantemplate"
target_dir="${output_dir%/}/${project_name}"
target_db_name="$(echo "$project_name" | tr '[:upper:]' '[:lower:]' | tr '.' '_' )"

mkdir -p "$target_dir"

tar -cf - \
  --exclude=".git" \
  --exclude="**/bin" \
  --exclude="**/obj" \
  --exclude=".vs" \
  . | (cd "$target_dir" && tar -xf -)

python3 - <<'PY' "$target_dir" "$template_name" "$project_name" "$template_db_name" "$target_db_name"
from pathlib import Path
import sys

root = Path(sys.argv[1])
old_name = sys.argv[2]
new_name = sys.argv[3]
old_db_name = sys.argv[4]
new_db_name = sys.argv[5]

paths = sorted(root.rglob("*"), key=lambda p: len(p.parts), reverse=True)
for path in paths:
    if old_name in path.name:
        path.rename(path.with_name(path.name.replace(old_name, new_name)))

for path in root.rglob("*"):
    if path.is_file():
        try:
            content = path.read_text(encoding="utf-8")
        except UnicodeDecodeError:
            continue

        updated = content.replace(old_name, new_name).replace(old_db_name, new_db_name)
        if updated != content:
            path.write_text(updated, encoding="utf-8")
PY

echo "Template scaffolded at: $target_dir"
