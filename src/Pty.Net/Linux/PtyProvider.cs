// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Pty.Net.Linux
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using static Pty.Net.Linux.NativeMethods;

    /// <summary>
    /// Provides a pty connection for linux machines.
    /// </summary>
    internal class PtyProvider : Unix.PtyProvider
    {
        /// <inheritdoc/>
        public override Task<IPtyConnection> StartTerminalAsync(PtyOptions options, TraceSource trace, CancellationToken cancellationToken)
        {
            var winSize = new Unix.PtyShim.PtyWinSize
            {
                Rows = (ushort)options.Rows,
                Cols = (ushort)options.Cols,
            };

            string?[] argv = GetExecvpArgs(options);
            string?[]? envp = GetEnvp(options.Environment);

            var cc = new byte[32];
            cc[(int)TermSpecialControlCharacter.VEOF] = 4;
            cc[(int)TermSpecialControlCharacter.VEOL] = unchecked((byte)(sbyte)-1);
            cc[(int)TermSpecialControlCharacter.VEOL2] = unchecked((byte)(sbyte)-1);
            cc[(int)TermSpecialControlCharacter.VERASE] = 0x7f;
            cc[(int)TermSpecialControlCharacter.VWERASE] = 23;
            cc[(int)TermSpecialControlCharacter.VKILL] = 21;
            cc[(int)TermSpecialControlCharacter.VREPRINT] = 18;
            cc[(int)TermSpecialControlCharacter.VINTR] = 3;
            cc[(int)TermSpecialControlCharacter.VQUIT] = 0x1c;
            cc[(int)TermSpecialControlCharacter.VSUSP] = 26;
            cc[(int)TermSpecialControlCharacter.VSTART] = 17;
            cc[(int)TermSpecialControlCharacter.VSTOP] = 19;
            cc[(int)TermSpecialControlCharacter.VLNEXT] = 22;
            cc[(int)TermSpecialControlCharacter.VDISCARD] = 15;
            cc[(int)TermSpecialControlCharacter.VMIN] = 1;
            cc[(int)TermSpecialControlCharacter.VTIME] = 0;

            var termios = new Unix.PtyShim.PtyTermios
            {
                IFlag = (uint)(TermInputFlag.ICRNL | TermInputFlag.IXON | TermInputFlag.IXANY | TermInputFlag.IMAXBEL | TermInputFlag.BRKINT | TermInputFlag.IUTF8),
                OFlag = (uint)(TermOuptutFlag.OPOST | TermOuptutFlag.ONLCR),
                CFlag = (uint)(TermConrolFlag.CREAD | TermConrolFlag.CS8 | TermConrolFlag.HUPCL),
                LFlag = (uint)(TermLocalFlag.ICANON | TermLocalFlag.ISIG | TermLocalFlag.IEXTEN | TermLocalFlag.ECHO | TermLocalFlag.ECHOE | TermLocalFlag.ECHOK | TermLocalFlag.ECHOKE | TermLocalFlag.ECHOCTL),
                CC = cc,
                ISpeed = (uint)TermSpeed.B38400,
                OSpeed = (uint)TermSpeed.B38400,
            };

            var result = Unix.PtyShim.pty_spawn(options.App, argv, envp, options.Cwd, ref termios, ref winSize);

            if (result.Pid == -1)
            {
                throw new InvalidOperationException($"pty_spawn failed with error {result.Error}");
            }

            return Task.FromResult<IPtyConnection>(new PtyConnection(result.MasterFd, result.Pid));
        }
    }
}
