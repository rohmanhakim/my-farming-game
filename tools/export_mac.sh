#!/usr/bin/env bash
# Exports the macOS app (universal) and makes it open on other Macs (2026-11-21).
# Usage: tools/export_mac.sh   (from the repository root) → build/macos/Fields & Sunshine (Mac).zip
# Godot 4.7.2 writes the game name unescaped ("&") into one Info.plist line, which breaks the
# signature ("cannot be opened / move to trash"). This script fixes the plist, ad-hoc signs the
# whole bundle again, verifies it, and zips it with ditto so the signature survives.
set -euo pipefail
cd "$(dirname "$0")/.."
GODOT="${GODOT:-$(command -v godot || echo /Applications/Godot_mono.app/Contents/MacOS/Godot)}"
OUT=build/macos
APP="Fields & Sunshine.app"
mkdir -p "$OUT/tmp"
"$GODOT" --headless --path src --export-release "macOS" "../$OUT/Fields & Sunshine.zip"
ditto -x -k "$OUT/Fields & Sunshine.zip" "$OUT/tmp"
PLIST="$OUT/tmp/$APP/Contents/Info.plist"
# Escape any bare & (not already an entity) in the plist.
perl -pi -e 's/&(?!amp;|lt;|gt;|quot;|apos;|#)/&amp;/g' "$PLIST"
plutil -lint "$PLIST"
codesign --force --deep -s - "$OUT/tmp/$APP"
codesign --verify --deep --strict "$OUT/tmp/$APP"
ditto -c -k --keepParent "$OUT/tmp/$APP" "$OUT/Fields & Sunshine (Mac).zip"
echo "Done: $OUT/Fields & Sunshine (Mac).zip (on the other Mac: xattr -cr the app, then right-click → Open)"
