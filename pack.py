#!/usr/bin/env python3
"""Pack and publish bradenacurtis801.Pty.Net to NuGet."""

import argparse
import glob
import os
import subprocess
import sys
import xml.etree.ElementTree as ET

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
CSPROJ = os.path.join(SCRIPT_DIR, "src", "Pty.Net", "Pty.Net.csproj")


def load_env():
    env_path = os.path.join(SCRIPT_DIR, ".env")
    if not os.path.exists(env_path):
        return
    with open(env_path) as f:
        for line in f:
            line = line.strip()
            if line and not line.startswith("#") and "=" in line:
                key, _, value = line.partition("=")
                os.environ.setdefault(key.strip(), value.strip())


def get_version():
    tree = ET.parse(CSPROJ)
    el = tree.getroot().find(".//Version")
    return el.text.strip() if el is not None else None


def set_version(version):
    tree = ET.parse(CSPROJ)
    root = tree.getroot()
    el = root.find(".//Version")
    if el is None:
        raise RuntimeError("<Version> element not found in csproj")
    el.text = version
    ET.indent(tree, space="  ")
    tree.write(CSPROJ, encoding="unicode", xml_declaration=False)
    print(f"Version updated to {version}")


def run(cmd, **kwargs):
    print(f"+ {' '.join(cmd)}")
    result = subprocess.run(cmd, **kwargs)
    if result.returncode != 0:
        sys.exit(result.returncode)


def bump_patch(version):
    parts = version.split(".")
    parts[-1] = str(int(parts[-1]) + 1)
    return ".".join(parts)


def main():
    parser = argparse.ArgumentParser(description="Pack and optionally publish Pty.Net")
    parser.add_argument("--publish", action="store_true", help="Push to NuGet after packing")
    parser.add_argument("--api-key", help="NuGet API key (required for --publish)")
    parser.add_argument("--version", help="Override version (e.g. 1.2.3)")
    parser.add_argument("--bump", action="store_true", help="Auto-increment patch version before packing")
    args = parser.parse_args()

    load_env()
    api_key = args.api_key or os.environ.get("NUGET_KEY")

    current = get_version()
    print(f"Current version: {current}")

    if args.version:
        set_version(args.version)
        version = args.version
    elif args.bump:
        version = bump_patch(current)
        set_version(version)
    else:
        version = current

    print(f"Packing version: {version}")

    run(["dotnet", "pack", CSPROJ, "-c", "Release"], cwd=SCRIPT_DIR)

    # Search all likely output locations — Directory.Build.props may redirect output
    search_roots = [
        os.path.join(SCRIPT_DIR, "bin", "Packages", "Release", "NuGet"),
        os.path.join(SCRIPT_DIR, "bin", "Pty.Net", "Release"),
    ]
    matches = []
    for root in search_roots:
        matches.extend(glob.glob(os.path.join(root, "bradenacurtis801.Pty.Net.*.nupkg")))

    if not matches:
        print("ERROR: could not find any bradenacurtis801.Pty.Net.*.nupkg", file=sys.stderr)
        print(f"Searched: {search_roots}", file=sys.stderr)
        sys.exit(1)

    nupkg = max(matches, key=os.path.getmtime)
    actual_version = os.path.basename(nupkg).removeprefix("bradenacurtis801.Pty.Net.").removesuffix(".nupkg")
    print(f"Package: {nupkg} (version {actual_version})")

    if args.publish:
        if not api_key:
            print("ERROR: no API key — set NUGET_KEY in .env or pass --api-key", file=sys.stderr)
            sys.exit(1)
        run([
            "dotnet", "nuget", "push", nupkg,
            "--api-key", api_key,
            "--source", "https://api.nuget.org/v3/index.json",
            "--skip-duplicate",
        ])
        print(f"Published bradenacurtis801.Pty.Net {actual_version}")
    else:
        print(f"\nTo publish, run:")
        print(f"  python3 pack.py --publish")
        print(f"  (or add --bump to auto-increment the patch version next time)")


if __name__ == "__main__":
    main()
