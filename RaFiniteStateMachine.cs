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
				States[i].Init(Parent);
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
			ExitCurrentState(false);

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
			ExitCurrentState(true);
			CurrentStateIndex = index;
			if(TryGetCurrentState(out var currentState))
			{
				currentState.Enter();
			}
		}

		private void ExitCurrentState(bool isSwitch)
		{
			if(TryGetCurrentState(out var currentState))
			{
				CurrentStateIndex = NO_STATE_INDEX;
				currentState.Exit(isSwitch);
			}
		}

		private bool TryGetCurrentState(out RaStateBase<TParent> currentState)
		{
			if(CurrentStateIndex >= 0 && CurrentStateIndex < States.Length)
			{
				currentState = States[CurrentStateIndex];
				return true;
			}
			currentState = default;
			return false;
		}
	}
}