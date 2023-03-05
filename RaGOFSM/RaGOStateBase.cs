using System;
using UnityEngine;

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
		protected T GetDependency<T>(bool searchHierarchy = true)
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
	}
}