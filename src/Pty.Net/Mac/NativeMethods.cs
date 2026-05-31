// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Pty.Net.Mac
{
    using System;

    /// <summary>
    /// Defines native types and methods for interop with Mac OS system APIs.
    /// </summary>
    internal static class NativeMethods
    {
        internal const int STDIN_FILENO = 0;
        internal const int SIGHUP = 1;

        public enum TermSpeed : uint
        {
            B38400 = 38400,
        }

        [Flags]
        public enum TermInputFlag : uint
        {
            /// <summary>
            /// Map BREAK to SIGINTR
            /// </summary>
            BRKINT = 0x00000002,

            /// <summary>
            /// Map CR to NL (ala CRMOD)
            /// </summary>
            ICRNL = 0x00000100,

            /// <summary>
            /// Enable output flow control
            /// </summary>
            IXON = 0x00000200,

            /// <summary>
            /// Any char will restart after stop
            /// </summary>
            IXANY = 0x00000800,

            /// <summary>
            /// Ring bell on input queue full
            /// </summary>
            IMAXBEL = 0x00002000,

            /// <summary>
            /// Maintain state for UTF-8 VERASE
            /// </summary>
            IUTF8 = 0x00004000,
        }

        [Flags]
        public enum TermOuptutFlag : uint
        {
            /// <summary>
            /// No output processing
            /// </summary>
            NONE = 0,

            /// <summary>
            /// Enable following output processing
            /// </summary>
            OPOST = 0x00000001,

            /// <summary>
            /// Map NL to CR-NL (ala CRMOD)
            /// </summary>
            ONLCR = 0x00000002,

            /// <summary>
            /// Map CR to NL
            /// </summary>
            OCRNL = 0x00000010,

            /// <summary>
            /// Don't output CR
            /// </summary>
            ONLRET = 0x00000040,
        }

        [Flags]
        public enum TermConrolFlag : uint
        {
            /// <summary>
            /// 8 bits
            /// </summary>
            CS8 = 0x00000300,

            /// <summary>
            /// Enable receiver
            /// </summary>
            CREAD = 0x00000800,

            /// <summary>
            /// Hang up on last close
            /// </summary>
            HUPCL = 0x00004000,
        }

        [Flags]
        public enum TermLocalFlag : uint
        {
            /// <summary>
            /// Visual erase for line kill
            /// </summary>
            ECHOKE = 0x00000001,

            /// <summary>
            /// Visually erase chars
            /// </summary>
            ECHOE = 0x00000002,

            /// <summary>
            /// Echo NL after line kill
            /// </summary>
            ECHOK = 0x00000004,

            /// <summary>
            /// Enable echoing
            /// </summary>
            ECHO = 0x00000008,

            /// <summary>
            /// Echo control chars as ^(Char)
            /// </summary>
            ECHOCTL = 0x00000040,

            /// <summary>
            /// Enable signals INTR, QUIT, [D]SUSP
            /// </summary>
            ISIG = 0x00000080,

            /// <summary>
            /// Canonicalize input lines
            /// </summary>
            ICANON = 0x00000100,

            /// <summary>
            /// Enable DISCARD and LNEXT
            /// </summary>
            IEXTEN = 0x00000400,
        }

        public enum TermSpecialControlCharacter
        {
            VEOF = 0,
            VEOL = 1,
            VEOL2 = 2,
            VERASE = 3,
            VWERASE = 4,
            VKILL = 5,
            VREPRINT = 6,
            VINTR = 8,
            VQUIT = 9,
            VSUSP = 10,
            VDSUSP = 11,
            VSTART = 12,
            VSTOP = 13,
            VLNEXT = 14,
            VDISCARD = 15,
            VMIN = 16,
            VTIME = 17,
            VSTATUS = 18,
        }
    }
}
