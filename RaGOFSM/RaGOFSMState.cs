using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static RaFSM.RaGOStateBase;

namespace RaFSM
{
	public class RaGOFSMState : RaGOStateBase, IRaFSMState
	{
		[Header("RaGOFSMState")]
		[SerializeField]
		private RaGOStateBase[] _states = null;

		[SerializeField]
		private bool _wrapFSM = false;

		[SerializeField]
		private bool _autoFillStates = true;

		[FormerlySerializedAs("_goFSMStateEvents")]
		public GoFSMStateEventCollection GoFSMStateEvents;

		private RaGOFiniteStateMachine _fsm = null;
		private IFSMGOCallbackReceiver _callbackReceiver = null;

		protected override void OnInit()
		{
			_fsm = new RaGOFiniteStateMachine(this, _states);
			try
			{
				_callbackReceiver = GetDependency<IFSMGOCallbackReceiver>();
			}
			catch
			{
				_callbackReceiver = default;
			}
		}

		protected override void OnEnter()
		{
			SwitchState(0);
		}

		protected override void OnDeinit()
		{
			_callbackReceiver = null;
			_fsm.Dispose();
			_fsm = null;

		}

		protected override void OnExit(bool isSwitch)
		{
			_fsm.SwitchState(null);
		}

		public void SwitchState(RaGOStateBase state)
		{
			RaGOStateBase oldState = (RaGOStateBase)_fsm.GetCurrentState();
			int newStateIndex =_fsm.SwitchState(state);
			FireNewStateEvent(newStateIndex, oldState);
		}

		public void SwitchState(int index)
		{
			RaGOStateBase oldState = (RaGOStateBase)_fsm.GetCurrentState();
			_fsm.SwitchState(index);
			FireNewStateEvent(index, oldState);
		}

		public void GoToNextState()
		{
			RaGOStateBase oldState = (RaGOStateBase)_fsm.GetCurrentState();
			int nextIndex = _fsm.GoToNextState(_wrapFSM);
			FireNewStateEvent(nextIndex, oldState);

			if(oldState != null && nextIndex == RaGOFiniteStateMachine.NO_STATE_INDEX)
			{
				GoFSMStateEvents.LastStateExitEvent.Invoke();
			}
		}

		protected void OnValidate()
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
			
			if(_callbackReceiver != null)
			{
				_callbackReceiver.OnStateSwitched(newState, oldState);
			}

			GoFSMStateEvents.SwitchedStateEvent.Invoke(newState, oldState);
			GoFSMStateEvents.SetStateEvent.Invoke(newState);
		}

		public interface IFSMGOCallbackReceiver
		{
			void OnStateSwitched(RaGOStateBase newState, RaGOStateBase oldState);
		}
	}

	[System.Serializable]
	public struct GoFSMStateEventCollection
	{
		public StateEvent SetStateEvent;
		public SwitchStateEvent SwitchedStateEvent;
		public UnityEvent LastStateExitEvent;
	}

	[System.Serializable]
	public class SwitchStateEvent : UnityEvent<RaGOStateBase, RaGOStateBase>
	{

	}
}