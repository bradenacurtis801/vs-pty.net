// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Pty.Net.Linux
{
    using System;

    internal static class NativeMethods
    {
        internal const int STDIN_FILENO = 0;
        internal const int SIGHUP = 1;

        public enum TermSpeed : uint
        {
            B38400 = 0x0F,
        }

        [Flags]
        public enum TermInputFlag : uint
        {
            BRKINT = 0x2,
            ICRNL = 0x100,
            IXON = 0x400,
            IXANY = 0x800,
            IMAXBEL = 0x2000,
            IUTF8 = 0x4000,
        }

        [Flags]
        public enum TermOuptutFlag : uint
        {
            OPOST = 1,
            ONLCR = 4,
        }

        [Flags]
        public enum TermConrolFlag : uint
        {
            CS8 = 0x30,
            CREAD = 0x80,
            HUPCL = 0x400,
        }

        [Flags]
        public enum TermLocalFlag : uint
        {
            ECHOKE = 0x800,
            ECHOE = 0x10,
            ECHOK = 0x20,
            ECHO = 0x8,
            ECHOCTL = 0x200,
            ISIG = 0x1,
            ICANON = 0x2,
            IEXTEN = 0x8000,
        }

        public enum TermSpecialControlCharacter
        {
            VEOF = 4,
            VEOL = 11,
            VEOL2 = 16,
            VERASE = 2,
            VWERASE = 14,
            VKILL = 3,
            VREPRINT = 12,
            VINTR = 0,
            VQUIT = 1,
            VSUSP = 10,
            VSTART = 8,
            VSTOP = 9,
            VLNEXT = 15,
            VDISCARD = 13,
            VMIN = 6,
            VTIME = 5,
        }
    }
}
