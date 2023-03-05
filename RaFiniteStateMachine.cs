using System;

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

			for(int i = States.Length - 1; i >= 0; i--)
			{
				var state = States[i];
				state.Deinit();
			}

			States = null;
			Parent = default;
		}

		public void GoToNextState(bool wrap)
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
		}

		public void SwitchState(RaStateBase<TParent> state)
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
					throw new Exception($"[{nameof(RaFiniteStateMachine<TParent>.SwitchState)}]: State `{state}` not part of StateMachine");
				}
			}
			else
			{
				SwitchState(NO_STATE_INDEX);
			}
		}

		public void SwitchState(int index)
		{
			EnterCurrentState(index);
		}

		private void EnterCurrentState(int index)
		{
			// Prepare Exit
			if(TryGetCurrentState(out var currentState))
			{
				currentState.PreSwitch();
			}

			// Prepare Enter
			if(TryGetState(index, out var nextState))
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
			CurrentStateIndex = index;
			if(nextState != null)
			{
				nextState.Enter();
			}
		}

		private bool TryGetCurrentState(out RaStateBase<TParent> currentState)
		{
			return TryGetState(CurrentStateIndex, out currentState);
		}

		private bool TryGetState(int index, out RaStateBase<TParent> state)
		{
			if(index >= 0 && index < States.Length)
			{
				state = States[index];
				return true;
			}
			state = default;
			return false;
		}
	}
}