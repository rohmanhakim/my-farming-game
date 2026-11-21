#!/usr/bin/env bash
# Windowed performance check (Phase-001 Task 1.20, project-structure.md §16).
# Usage: tools/perf_check.sh            (from the repository root; runs the pc and handheld profiles)
# Needs a real window (not headless). Exit 0 = both profiles inside the budget.
set -uo pipefail
cd "$(dirname "$0")/.."
GODOT="${GODOT:-$(command -v godot || echo /Applications/Godot_mono.app/Contents/MacOS/Godot)}"
dotnet build src/MyFarmingGame.csproj -nologo -v q >/dev/null || exit 1
status=0
for profile in pc handheld; do
  "$GODOT" --path src res://debug/PerfCheck.tscn -- --profile="$profile" 2>&1 | grep "PERF" || status=1
  [ "${PIPESTATUS[0]}" -eq 0 ] || status=1
done
exit $status
