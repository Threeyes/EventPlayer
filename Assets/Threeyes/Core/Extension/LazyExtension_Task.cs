
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Ref:https://forum.unity.com/threads/cancel-async-task-best-practice.805839/
/// 
/// ToAdd:http://www.stevevermeulen.com/index.php/2017/09/using-async-await-in-unity3d-2017/
/// </summary>
public static class LazyExtension_Task
{
    public static void TryCancelAndDispose(this CancellationTokenSource tokenSource)
    {
        if (tokenSource != null)
        {
            tokenSource.Cancel();
            tokenSource.Dispose();
        }
    }

    /// <summary>
    /// 停掉之前未完成的Task，并返回新的CancellationTokenSource，便于重新开始新Task
    /// 
    /// 使用方式：c=c.RefreshToken()
    /// </summary>
    /// <param name="tokenSource"></param>
    /// <returns></returns>
    public static CancellationTokenSource RefreshToken(this CancellationTokenSource tokenSource)
    {
        //Cancel Previous
        tokenSource?.Cancel();
        tokenSource?.Dispose();

        return new CancellationTokenSource();
    }

    /// <summary>
    /// 
    /// Ref:https://stackoverflow.com/questions/25632803/how-to-transform-task-waitcancellationtoken-to-an-await-statement
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
        return task.ContinueWith(t => t.GetAwaiter().GetResult(), cancellationToken);
    }

    public static Task WithCancellation(this Task task, CancellationToken cancellationToken)
    {
        return task.ContinueWith(t => t.GetAwaiter(), cancellationToken);
    }
}