using System.Threading.Tasks;

public static class TaskWaiters
{

    /// <summary>Awaits the next frame.</summary>
    public static Task NextFrame
    {
        get
        {

            var tcs = new TaskCompletionSource<bool>();

            AIAgent.UpdateHelper.OnUpdate += OnUpdate;
            void OnUpdate()
            {
                tcs.TrySetResult(true);
                AIAgent.UpdateHelper.OnUpdate -= OnUpdate;
            }

            return tcs.Task;

        }
    }

}