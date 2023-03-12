using System;
using System.Collections.Generic;

namespace RaFSM
{
	public class RaFiniteStateMachine<TParent> : IDisposable
	{
		public const int NO_STATE_INDEX = -1;

		public RaStateBase<TParent>[] States
		{
			get; private set;
		}

		public TParent Parent
		{
			get; private set;
		}

		public int CurrentStateIndex
		{
			get; private set;
		}

		private int? _currentSwitch = null;
		private Queue<int> _requestedSwitches = new Queue<int>();

		public RaFiniteStateMachine(TParent parent, RaStateBase<TParent>[] states)
		{
			CurrentStateIndex = NO_STATE_INDEX;
			States = states;
			Parent = parent;

			for(int i = 0; i < States.Length; i++)
			{
				var state = States[i];
				if(state.IsInitialized)
				{
					throw new Exception($"[{nameof(RaFiniteStateMachine<TParent>)}]: State `{state}` already initialized. Did you add the same state to different Finite State Machines?");
				}
				state.Init(Parent);
			}
		}

		public RaStateBase<TParent> GetCurrentState()
		{
			if(TryGetCurrentState(out var currentState) && currentState.IsCurrentState)
			{
				return currentState;
			}
			return default;
		}

		public void Dispose()
		{
			if(TryGetCurrentState(out var currentState))
			{
				CurrentStateIndex = NO_STATE_INDEX;
				currentState.Exit(false);
			}

			_requestedSwitches.Clear();
			_currentSwitch = null;

			for(int i = States.Length - 1; i >= 0; i--)
			{
				var state = States[i];
				state.Deinit();
			}

			States = null;
			Parent = default;
		}

		public int GoToNextState(bool wrap)
		{
			int nextIndex = CurrentStateIndex + 1;
			if(nextIndex >= States.Length)
			{
				if(wrap)
				{
					nextIndex = 0;
				}
				else
				{
					nextIndex = NO_STATE_INDEX;
				}
			}

			SwitchState(nextIndex);
			return nextIndex;
		}

		public int SwitchState(RaStateBase<TParent> state)
		{
			if(state != null)
			{
				int index = Array.IndexOf(States, state);
				if(index >= 0)
				{
					SwitchState(index);
				}
				else
				{
					throw new Exception($"[{nameof(RaFiniteStateMachine<TParent>.SwitchState)}]: State `{state}` not part of StateMachine `{Parent}`");
				}
				return index;
			}
			else
			{
				SwitchState(NO_STATE_INDEX);
				return NO_STATE_INDEX;
			}
		}

		public void SwitchState(int index)
		{
			EnterCurrentState(index);
		}

		public bool TryGetCurrentState(out RaStateBase<TParent> currentState)
		{
			return TryGetState(CurrentStateIndex, out currentState);
		}

		public bool TryGetState(int index, out RaStateBase<TParent> state)
		{
			if(index >= 0 && index < States.Length)
			{
				state = States[index];
				return true;
			}
			state = default;
			return false;
		}

		private void EnterCurrentState(int index)
		{
			_requestedSwitches.Enqueue(index);

			if(_currentSwitch.HasValue)
			{
#if RAFSM_DEBUG
				UnityEngine.Debug.LogWarning($"Switch to {index} requested while switching to {_currentSwitch.Value}. Added {index} to queue");
#endif
				return;
			}

			while(_requestedSwitches.Count > 0)
			{
				_currentSwitch = _requestedSwitches.Dequeue();
				// Prepare Exit
				if(TryGetCurrentState(out var currentState))
				{
					currentState.PreSwitch();
				}

				// Prepare Enter
				if(TryGetState(_currentSwitch.Value, out var nextState))
				{
					nextState.PreSwitch();
				}

				// Exit
				CurrentStateIndex = NO_STATE_INDEX;
				if(currentState != null)
				{
					currentState.Exit(true);
				}

				// Enter
				CurrentStateIndex = _currentSwitch.Value;
				if(nextState != null)
				{
					nextState.Enter();
				}
			}
			_currentSwitch = null;
		}
	}
}