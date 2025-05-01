namespace StateMachine.Example;

public enum NetworkState
{
    Stopped,
    Connecting,
    Ready,
    GotError,
    Stopping,
}