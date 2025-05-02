using StateMachine.StateMachineBase;

namespace StateMachine.Example;

public class NetworkStateMachineDescription 
    : StateMachineDescription<NetworkState, NetworkContext, NetworkStateMachineDescription> 
{ }