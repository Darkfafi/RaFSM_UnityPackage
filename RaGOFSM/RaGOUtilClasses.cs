using System;
using UnityEngine.Events;

namespace RaFSM.Core
{
	[Serializable]
	public struct RaGOCoreEventCollection
	{
		public RaGOStateEvent InitStateEvent;
		public RaGOStateEvent EnterStateEvent;
		public RaGOStateEvent ExitStateEvent;
		public RaGOStateEvent DeinitStateEvent;
	}


	[Serializable]
	public struct RaGOCoreEditorOptionCollection
	{
		public string CurrentStateSuffix;
	}

	[Serializable]
	public struct RaGoFSMEventCollection
	{
		public RaGOStateEvent SetStateEvent;
		public RaGOSwitchStateEvent SwitchedStateEvent;
		public UnityEvent LastStateExitEvent;
	}
}

namespace RaFSM
{
	[Serializable]
	public class RaGOSwitchStateEvent : UnityEvent<RaGOStateBase, RaGOStateBase>
	{

	}

	[Serializable]
	public class RaGOStateEvent : UnityEvent<RaGOStateBase>
	{

	}

	public interface IRaGOFSMCallbackReceiver
	{
		void OnStateSwitched(RaGOStateBase newState, RaGOStateBase oldState);
	}

	public interface IRaGOFSM : IRaFSMCycler
	{
		void SwitchState(RaGOStateBase state);
		void SwitchState(int index);
	}

	public interface IRaFSMCycler
	{
		void GoToNextState();
	}
}