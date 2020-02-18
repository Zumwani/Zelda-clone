using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AIAgent : MonoBehaviour
{

    public enum StartMode
    {
        Start, Awake, OnEnable
    }

    public StartMode startAt;
    public AIBehaviour behaviour;

    [ToggleButton(nameof(isRunning), false, "Resume", "Pause", nameof(OnButton))]
    public bool button;

    private void Start() =>     StartBehaviour();
    private void Awake() =>     StartBehaviour();
    private void OnEnable() =>  StartBehaviour();

    [Label]
    public string action;

    bool isRunning;
    CancellationTokenSource cancellationToken;

    void StartBehaviour([CallerMemberName] string caller = "")
    {

        if (caller != startAt.ToString())
            return;

        Resume();

    }

    public void Pause()
    {
        isRunning = false;
        cancellationToken.Cancel();
    }

    void OnButton(bool value)
    {
        if (value)
            Resume();
        else
            Pause();
    }

    public async void Resume()
    {

        if (isRunning)
            return;

        isRunning = true;
        cancellationToken = new CancellationTokenSource();
        while (isRunning)
           await behaviour.actions.Evaluate(this, cancellationToken.Token);

    }

    private void Update()
    {
        UpdateHelper.Update();
    }

    internal static class UpdateHelper
    {

        public delegate void onUpdate();
        public static onUpdate OnUpdate;

        public static void Update() =>
            OnUpdate?.Invoke();

    }

    public Task NextFrame => TaskWaiters.NextFrame;

}
