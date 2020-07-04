using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public static class TaskUtils
{
    public static Task WaitForSeconds(MonoBehaviour target, float seconds)
    {
        return WaitForYieldInstruction(target, new WaitForSeconds(seconds));
    }

    public static Task WaitForSecondsRealtime(MonoBehaviour target, float seconds)
    {
        return WaitForYieldInstruction(target, new WaitForSecondsRealtime(seconds));
    }

    public static Task WaitForNextUpdate(MonoBehaviour target)
    {
        return WaitForYieldInstruction(target, null);
    }

    public static Task WaitForYieldInstruction(MonoBehaviour target, object yieldInstruction)
    {
        TaskCompletionSource<bool> taskSource = new TaskCompletionSource<bool>();
        target.StartCoroutine(WaitForSecondsAux(yieldInstruction, taskSource));
        return taskSource.Task;
    }

    private static IEnumerator WaitForSecondsAux(object yieldInstruction, TaskCompletionSource<bool> taskSource)
    {
        yield return yieldInstruction;
        taskSource.SetResult(true);
    }
}
