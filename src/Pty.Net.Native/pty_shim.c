/*
 * pty_shim.c - Native PTY shim for Pty.Net
 *
 * Wraps forkpty() + execvp() in native code to avoid W^X (Write XOR Execute)
 * memory protection issues when forking from managed .NET 7+ code. The .NET
 * JIT marks pages non-executable; any managed code running in the child after
 * fork will crash because it touches JIT'd memory. By doing fork+exec entirely
 * here — with no managed code in the child path — that window is eliminated.
 *
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT license.
 */

#if defined(__APPLE__)
    #include <util.h>
    #include <sys/ioctl.h>
#else
    #include <pty.h>
#endif

#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <errno.h>
#include <termios.h>
#include <sys/wait.h>
#include <signal.h>

#if defined(_WIN32)
    #define PTY_EXPORT __declspec(dllexport)
#else
    #define PTY_EXPORT __attribute__((visibility("default")))
#endif

typedef struct {
    unsigned int c_iflag;
    unsigned int c_oflag;
    unsigned int c_cflag;
    unsigned int c_lflag;
    unsigned char c_cc[32];
    unsigned int c_ispeed;
    unsigned int c_ospeed;
} pty_termios_t;

typedef struct {
    unsigned short ws_row;
    unsigned short ws_col;
    unsigned short ws_xpixel;
    unsigned short ws_ypixel;
} pty_winsize_t;

typedef struct {
    int master_fd;
    int pid;
    int error;
} pty_spawn_result_t;

/*
 * Spawns a new process attached to a pseudo-terminal.
 * fork+exec runs entirely in native code — no managed .NET code executes in
 * the child, which avoids W^X crashes on .NET 7+.
 */
PTY_EXPORT pty_spawn_result_t pty_spawn(
    const char* file,
    char* const argv[],
    char* const envp[],
    const char* working_dir,
    const pty_termios_t* termios_settings,
    const pty_winsize_t* winsize_settings)
{
    pty_spawn_result_t result = { -1, -1, 0 };

    struct termios term;
    struct termios* term_ptr = NULL;
    if (termios_settings != NULL) {
        memset(&term, 0, sizeof(term));
        term.c_iflag = termios_settings->c_iflag;
        term.c_oflag = termios_settings->c_oflag;
        term.c_cflag = termios_settings->c_cflag;
        term.c_lflag = termios_settings->c_lflag;
        size_t cc_size = sizeof(term.c_cc);
        if (cc_size > 32) cc_size = 32;
        memcpy(term.c_cc, termios_settings->c_cc, cc_size);
        cfsetispeed(&term, termios_settings->c_ispeed);
        cfsetospeed(&term, termios_settings->c_ospeed);
        term_ptr = &term;
    }

    struct winsize ws;
    struct winsize* ws_ptr = NULL;
    if (winsize_settings != NULL) {
        ws.ws_row = winsize_settings->ws_row;
        ws.ws_col = winsize_settings->ws_col;
        ws.ws_xpixel = winsize_settings->ws_xpixel;
        ws.ws_ypixel = winsize_settings->ws_ypixel;
        ws_ptr = &ws;
    }

    int master_fd = -1;
    pid_t pid = forkpty(&master_fd, NULL, term_ptr, ws_ptr);

    if (pid == -1) {
        result.error = errno;
        return result;
    }

    if (pid == 0) {
        /* Child — no managed code runs here */
        if (working_dir != NULL && working_dir[0] != '\0') {
            if (chdir(working_dir) == -1) _exit(errno);
        }
        if (envp != NULL) {
            for (int i = 0; envp[i] != NULL; i++) {
                char* eq = strchr(envp[i], '=');
                if (eq != NULL) {
                    size_t key_len = eq - envp[i];
                    char* key = (char*)alloca(key_len + 1);
                    memcpy(key, envp[i], key_len);
                    key[key_len] = '\0';
                    const char* value = eq + 1;
                    if (value[0] == '\0') unsetenv(key);
                    else setenv(key, value, 1);
                }
            }
        }
        execvp(file, argv);
        _exit(errno);
    }

    result.master_fd = master_fd;
    result.pid = (int)pid;
    return result;
}

PTY_EXPORT int pty_resize(int master_fd, unsigned short rows, unsigned short cols)
{
    struct winsize ws;
    ws.ws_row = rows;
    ws.ws_col = cols;
    ws.ws_xpixel = 0;
    ws.ws_ypixel = 0;
    return ioctl(master_fd, TIOCSWINSZ, &ws);
}

PTY_EXPORT int pty_kill(int pid, int sig)
{
    return kill(pid, sig);
}

PTY_EXPORT int pty_waitpid(int pid, int* status, int options)
{
    return waitpid(pid, status, options);
}

PTY_EXPORT int pty_close(int master_fd)
{
    return close(master_fd);
}

PTY_EXPORT int pty_get_errno(void)
{
    return errno;
}
