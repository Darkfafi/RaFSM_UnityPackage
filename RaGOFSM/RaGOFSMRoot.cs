using RaFSM.Core;
using UnityEngine;

namespace RaFSM
{
	public sealed class RaGOFSMRoot : MonoBehaviour, IRaGOFSM
	{
		[Header("RaGOFSMRoot")]
		[SerializeField]
		private RaGOStateBase[] _states = null;

		[SerializeField]
		private bool _wrapFSM = false;

		[SerializeField]
		private bool _autoFillStates = true;

		[SerializeField]
		private bool _excludeDisabledStates = true;

		[SerializeField]
		private RunType _runType = RunType.Start;

		public RaGoFSMEventCollection GoFSMStateEvents;

		private RaGOFiniteStateMachine _fsm = null;

		public bool IsRunning => _fsm != null && _fsm.CurrentStateIndex != RaGOFiniteStateMachine.NO_STATE_INDEX;

		private void Awake()
		{
			_fsm = new RaGOFiniteStateMachine(this, _states, _excludeDisabledStates);

			if(_runType == RunType.Awake)
			{
				RunFSM();
			}
		}

		private void Start()
		{
			if(_runType == RunType.Start)
			{
				RunFSM();
			}
		}

		private void OnDestroy()
		{
			if(_fsm != null)
			{
				_fsm.Dispose();
				_fsm = null;
			}
		}

		public void RunFSM(int index = 0)
		{
			if(!IsRunning)
			{
				_fsm.SwitchState(index);
			}
		}

		public void SwitchState(RaGOStateBase state)
		{
			if(IsRunning)
			{
				RaGOStateBase oldState = (RaGOStateBase)_fsm.GetCurrentState();
				int newStateIndex = _fsm.SwitchState(state);
				FireNewStateEvent(newStateIndex, oldState);
			}
		}

		public void SwitchState(int index)
		{
			if(IsRunning)
			{
				RaGOStateBase oldState = (RaGOStateBase)_fsm.GetCurrentState();
				_fsm.SwitchState(index);
				FireNewStateEvent(index, oldState);
			}
		}

		public void GoToNextState()
		{
			if(IsRunning)
			{
				RaGOStateBase oldState = (RaGOStateBase)_fsm.GetCurrentState();
				int nextIndex = _fsm.GoToNextState(_wrapFSM);
				FireNewStateEvent(nextIndex, oldState);

				if(oldState != null && nextIndex == RaGOFiniteStateMachine.NO_STATE_INDEX)
				{
					GoFSMStateEvents.LastStateExitEvent.Invoke();
				}
			}
		}

		private void OnValidate()
		{
			if(_autoFillStates)
			{
				_states = RaGOFiniteStateMachine.GetGOStates(transform);
			}
		}

		private void FireNewStateEvent(int newStateIndex, RaGOStateBase oldState)
		{
			RaGOStateBase newState = null;
			if(_fsm.TryGetState(newStateIndex, out var newStateRaw))
			{
				newState = (RaGOStateBase)newStateRaw;
			}

			GoFSMStateEvents.SwitchedStateEvent.Invoke(newState, oldState);
			GoFSMStateEvents.SetStateEvent.Invoke(newState);
		}

		public enum RunType
		{
			Manual = 0,
			Awake = 1,
			Start = 2
		}
	}
}