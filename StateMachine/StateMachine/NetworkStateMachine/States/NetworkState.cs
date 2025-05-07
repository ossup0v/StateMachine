namespace StateMachine.NetworkStateMachine;

public enum NetworkState
{
    Stopped,
    Connecting,
    Ready,
    GotError,
    Stopping,
}