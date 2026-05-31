#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BUILD_DIR="$SCRIPT_DIR/build"
OUTPUT_DIR="$SCRIPT_DIR/output"

if [[ "$OSTYPE" == "darwin"* ]]; then
    LIB_EXT="dylib"
    for ARCH in "x86_64" "arm64"; do
        RID="osx-$([[ $ARCH == arm64 ]] && echo arm64 || echo x64)"
        ARCH_BUILD_DIR="$BUILD_DIR/$RID"
        rm -rf "$ARCH_BUILD_DIR" && mkdir -p "$ARCH_BUILD_DIR"
        cd "$ARCH_BUILD_DIR"
        cmake -DCMAKE_BUILD_TYPE=Release -DCMAKE_OSX_ARCHITECTURES="$ARCH" "$SCRIPT_DIR"
        cmake --build . --config Release
        RUNTIME_DIR="$OUTPUT_DIR/runtimes/$RID/native"
        mkdir -p "$RUNTIME_DIR"
        cp "$ARCH_BUILD_DIR/bin/libpty_net.$LIB_EXT" "$RUNTIME_DIR/"
        echo "Built: $RUNTIME_DIR/libpty_net.$LIB_EXT"
    done
elif [[ "$OSTYPE" == "linux"* ]]; then
    LIB_EXT="so"
    CURRENT_ARCH=$(uname -m)
    RID="linux-$([[ $CURRENT_ARCH == aarch64 ]] && echo arm64 || echo x64)"
    ARCH_BUILD_DIR="$BUILD_DIR/$RID"
    rm -rf "$ARCH_BUILD_DIR" && mkdir -p "$ARCH_BUILD_DIR"
    cd "$ARCH_BUILD_DIR"
    cmake -DCMAKE_BUILD_TYPE=Release "$SCRIPT_DIR"
    cmake --build . --config Release
    RUNTIME_DIR="$OUTPUT_DIR/runtimes/$RID/native"
    mkdir -p "$RUNTIME_DIR"
    cp "$ARCH_BUILD_DIR/bin/libpty_net.$LIB_EXT" "$RUNTIME_DIR/"
    echo "Built: $RUNTIME_DIR/libpty_net.$LIB_EXT"
else
    echo "Unsupported platform: $OSTYPE" && exit 1
fi

echo "Build complete. Output: $OUTPUT_DIR/runtimes/"
