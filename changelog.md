# Changelog RaFSM

## v1.3.1 - 17/03/2023
* Added SwitchedStateEvent to the RaFiniteStateMachine
* Added GetStateIndex method to the RaFiniteStateMachine
* Made it so the GetDependency<T> can be overwritten for custom Dependency logics
* Made Generic Variants for Dependency injection for the RaGoFSMState
* Made it so the RaGOFSMState no longer allows for state changing if it is not the current state

## v1.3.0 - 12/03/2023
* Added Exit last state event for GO FSM
* Added queue to RaFiniteStateMachine, when a state tries to change to the next state during a switch, handling them one at a time
* Added FSM_{X} Methods to allow a more generic state switch linking, automatically switching the states parent's FSM state
* Added Suffix naming integration so you can optionally see the Current state of the FSM by the change of names
* Made it so the Switch newState passed to events is the state which was initially requested to be switched to, instead of the current state
* Made the TryGetState / TryGetCurrentState getters public for RaFiniteStateMachine

## v1.2.1 - 11/03/2023
* Corrected FSM State Events accessibility

## v1.2.0 - 11/03/2023
* Made the UnityEvents and the GetDependency method publicly accessible

## v1.1.0 - 05/03/2023
* Added UnityEvents to the RaGOFSM cycle to create hooks for Enter, Exit, Set & Switch

## v1.0.0 - 05/03/2023
* Initial Release
