// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Pty.Net.Unix
{
    using System.Runtime.InteropServices;

    internal static class PtyShim
    {
        private const string LibName = "pty_net";

        [StructLayout(LayoutKind.Sequential)]
        internal struct PtyTermios
        {
            public uint IFlag;
            public uint OFlag;
            public uint CFlag;
            public uint LFlag;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] CC;
            public uint ISpeed;
            public uint OSpeed;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PtyWinSize
        {
            public ushort Rows;
            public ushort Cols;
            public ushort XPixel;
            public ushort YPixel;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PtySpawnResult
        {
            public int MasterFd;
            public int Pid;
            public int Error;
        }

        [DllImport(LibName)]
        internal static extern PtySpawnResult pty_spawn(
            [MarshalAs(UnmanagedType.LPStr)] string file,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string?[] argv,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string?[]? envp,
            [MarshalAs(UnmanagedType.LPStr)] string working_dir,
            ref PtyTermios termios,
            ref PtyWinSize winsize);

        [DllImport(LibName, SetLastError = true)]
        internal static extern int pty_resize(int masterFd, ushort rows, ushort cols);

        [DllImport(LibName, SetLastError = true)]
        internal static extern int pty_kill(int pid, int sig);

        [DllImport(LibName, SetLastError = true)]
        internal static extern int pty_waitpid(int pid, ref int status, int options);

        [DllImport(LibName, SetLastError = true)]
        internal static extern int pty_close(int masterFd);

        [DllImport(LibName)]
        internal static extern int pty_get_errno();
    }
}
