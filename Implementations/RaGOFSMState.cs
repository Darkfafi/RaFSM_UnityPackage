using UnityEngine;

namespace RaFSM
{
	public class RaGOFSMState : RaGOStateBase, IRaFSMState
	{
		[SerializeField]
		private RaGOStateBase[] _states = null;

		[SerializeField]
		private bool _wrapFSM = false;

		[SerializeField]
		private bool _autoFillStates = true;

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

		void IRaFSMState.GoToNextState()
		{
			RaGOStateBase oldState = (RaGOStateBase)_fsm.GetCurrentState();
			_fsm.GoToNextState(_wrapFSM);
			if(_callbackReceiver != null)
			{
				RaGOStateBase newState = (RaGOStateBase)_fsm.GetCurrentState();
				_callbackReceiver.OnStateSwitched(newState, oldState);
			}
		}

		protected void OnValidate()
		{
			if(_autoFillStates)
			{
				_states = RaGOFiniteStateMachine.GetGOStates(transform);
			}
		}

		public interface IFSMGOCallbackReceiver
		{
			void OnStateSwitched(RaGOStateBase newState, RaGOStateBase oldState);
		}
	}
}