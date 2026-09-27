using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fireman.Core.Utils;

/// <summary>
/// For running code on Unity's main thread.
/// </summary>
public static class UnityMainThreadDispatcher {
    private static SynchronizationContext _mainThreadContext = null!;

    public static void Initialize() {
        _mainThreadContext = SynchronizationContext.Current;
    }

    /// <summary>
    /// Executes on Unity's main thread
    /// </summary>
    public static Task RunOnMainThread(Action action) {
        var tcs = new TaskCompletionSource<bool>();
        _mainThreadContext.Post(_ => {
            try {
                action();
                tcs.SetResult(true);
            } catch (Exception ex) {
                tcs.SetException(ex);
            }
        }, null);
        return tcs.Task;
    }
}
