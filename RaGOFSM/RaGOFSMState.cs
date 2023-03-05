using UnityEngine;
using UnityEngine.Events;

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

		[SerializeField]
		private GoFSMStateEvents _goFSMStateEvents;

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
			_fsm.SwitchState(0);
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
			_fsm.SwitchState(state);
			FireNewStateEvent(oldState);
		}

		public void SwitchState(int index)
		{
			RaGOStateBase oldState = (RaGOStateBase)_fsm.GetCurrentState();
			_fsm.SwitchState(index);
			FireNewStateEvent(oldState);
		}

		public void GoToNextState()
		{
			RaGOStateBase oldState = (RaGOStateBase)_fsm.GetCurrentState();
			_fsm.GoToNextState(_wrapFSM);
			FireNewStateEvent(oldState);
		}

		protected void OnValidate()
		{
			if(_autoFillStates)
			{
				_states = RaGOFiniteStateMachine.GetGOStates(transform);
			}
		}

		private void FireNewStateEvent(RaGOStateBase oldState)
		{
			RaGOStateBase newState = (RaGOStateBase)_fsm.GetCurrentState();
			
			if(_callbackReceiver != null)
			{
				_callbackReceiver.OnStateSwitched(newState, oldState);
			}

			_goFSMStateEvents.SwitchedStateEvent.Invoke(newState, oldState);
			_goFSMStateEvents.SetStateEvent.Invoke(newState);
		}

		public interface IFSMGOCallbackReceiver
		{
			void OnStateSwitched(RaGOStateBase newState, RaGOStateBase oldState);
		}
	}

	[System.Serializable]
	public struct GoFSMStateEvents
	{
		public NewStateEvent SetStateEvent;
		public SwitchStateEvent SwitchedStateEvent;
	}

	[System.Serializable]
	public class SwitchStateEvent : UnityEvent<RaGOStateBase, RaGOStateBase>
	{

	}

	[System.Serializable]
	public class NewStateEvent : UnityEvent<RaGOStateBase>
	{
		
	}
}