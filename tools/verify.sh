#!/usr/bin/env bash
# Software checks for the Godot project (project-structure.md §11).
# Usage: tools/verify.sh                 (from the repository root; all test groups)
#        tools/verify.sh rules           (only these TestRunner groups: rules, game)
# Set GODOT to your Godot .NET binary if it is not on PATH.
set -euo pipefail
cd "$(dirname "$0")/.."
GODOT="${GODOT:-$(command -v godot || echo /Applications/Godot_mono.app/Contents/MacOS/Godot)}"

echo "== dotnet build"
dotnet build src/MyFarmingGame.csproj -nologo -v q
echo "== import"
"$GODOT" --headless --path src --import >/dev/null 2>&1
echo "== main scene (3D) boots (120 frames)"
"$GODOT" --headless --path src --quit-after 120
echo "== tests ${1:-all groups}"
if [ -n "${1:-}" ]; then
  "$GODOT" --headless --path src res://tests/TestRunner.tscn -- --group="$1"
else
  "$GODOT" --headless --path src res://tests/TestRunner.tscn
fi
echo "== git: no .godot/ files tracked or untracked"
if git status --porcelain --untracked-files=all | grep -q 'src/\.godot/'; then echo "FAIL: .godot/ visible to git"; exit 1; fi
echo "ALL CHECKS PASSED"
