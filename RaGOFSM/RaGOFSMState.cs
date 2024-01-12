using RaFSM.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace RaFSM
{
	public class RaGOFSMState<TDependencyA, TDependencyB> : RaGOFSMState
	{
		[SerializeField]
		private bool _searchHierarchyForDependencies = true;

		public TDependencyA DependencyA
		{
			get; private set;
		}

		public TDependencyB DependencyB
		{
			get; private set;
		}

		protected override void OnPreInitialize()
		{
			DependencyA = GetDependency<TDependencyA>(_searchHierarchyForDependencies);
			DependencyB = GetDependency<TDependencyB>(_searchHierarchyForDependencies);
		}
	}

	public class RaGOFSMState<TDependency> : RaGOFSMState
	{
		[SerializeField]
		private bool _searchHierarchyForDependencies = true;

		public TDependency Dependency
		{
			get; private set;
		}

		protected override void OnPreInitialize()
		{
			Dependency = GetDependency<TDependency>(_searchHierarchyForDependencies);
		}
	}

	public class RaGOFSMState : RaGOStateBase, IRaGOFSM
	{
		[Header("RaGOFSMState")]
		[SerializeField]
		private RaGOStateBase[] _states = null;

		[SerializeField]
		private bool _wrapFSM = false;

		[SerializeField]
		private bool _autoFillStates = true;

		[SerializeField]
		private bool _excludeDisabledStates = true;

		[FormerlySerializedAs("_goFSMStateEvents")]
		public RaGoFSMEventCollection GoFSMStateEvents;

		private RaGOFiniteStateMachine _fsm = null;
		private IRaGOFSMCallbackReceiver _callbackReceiver = null;

		public int CurrentStateIndex => _fsm != null ? _fsm.CurrentStateIndex : -1;

		protected override void OnInit()
		{
			_fsm = new RaGOFiniteStateMachine(this, _states, _excludeDisabledStates);
			try
			{
				_callbackReceiver = GetDependency<IRaGOFSMCallbackReceiver>();
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
			if(IsCurrentState)
			{
				RaGOStateBase oldState = (RaGOStateBase)_fsm.GetCurrentState();
				int newStateIndex = _fsm.SwitchState(state);
				FireNewStateEvent(newStateIndex, oldState);
			}
		}

		public void SwitchState(int index)
		{
			if(IsCurrentState)
			{
				RaGOStateBase oldState = (RaGOStateBase)_fsm.GetCurrentState();
				_fsm.SwitchState(index);
				FireNewStateEvent(index, oldState);
			}
		}

		public void GoToNextState()
		{
			if(IsCurrentState)
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

		protected virtual void OnValidate()
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
	}
}