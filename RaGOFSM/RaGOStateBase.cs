using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace RaFSM
{
	public abstract class RaGOStateBase<TDependencyA, TDependencyB> : RaGOStateBase
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

	public abstract class RaGOStateBase<TDependency> : RaGOStateBase
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

	public abstract class RaGOStateBase : RaStateBase<Component>
	{
		[Header("Core")]
		[FormerlySerializedAs("_coreEvents")]
		public CoreEventCollection CoreEvents;

		public CoreEditorOptionCollection CoreEditorOptions = new CoreEditorOptionCollection()
		{
			CurrentStateSuffix = " [CURRENT]",
		};

		public virtual T GetDependency<T>(bool searchHierarchy = true)
		{
			if(Parent is T castedParent)
			{
				return castedParent;
			}

			if(Parent.TryGetComponent(out T componentValue))
			{
				return componentValue;
			}

			if(searchHierarchy && Parent is RaGOStateBase goStateParent)
			{
				return goStateParent.GetDependency<T>(searchHierarchy);
			}

			throw new Exception($"[{nameof(RaGOStateBase.GetDependency)}]: Dependency of type '{typeof(T).Name}' not found on {Parent}!");
		}

		public void FSM_GoToNextState()
		{
			GetDependency<IRaFSMState>().GoToNextState();
		}

		public void FSM_SwitchState(int index)
		{
			GetDependency<RaGOFSMState>().SwitchState(index);
		}

		public void FSM_SwitchState(RaGOStateBase state)
		{
			GetDependency<RaGOFSMState>().SwitchState(state);
		}

		internal override bool Enter()
		{
			if(base.Enter())
			{
				name += CoreEditorOptions.CurrentStateSuffix;
				CoreEvents.EnterStateEvent.Invoke(this);
				return true;
			}
			return false;
		}

		internal override bool Exit(bool isSwitch)
		{
			if(base.Exit(isSwitch))
			{
				name = name.Replace(CoreEditorOptions.CurrentStateSuffix, string.Empty);
				CoreEvents.ExitStateEvent.Invoke(this);
				return true;
			}
			return false;
		}

		[Serializable]
		public struct CoreEventCollection
		{
			public StateEvent EnterStateEvent;
			public StateEvent ExitStateEvent;
		}


		[Serializable]
		public struct CoreEditorOptionCollection
		{
			public string CurrentStateSuffix;
		}

		[Serializable]
		public class StateEvent : UnityEvent<RaGOStateBase>
		{

		}
	}
}