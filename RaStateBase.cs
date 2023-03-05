using UnityEngine;

namespace RaFSM
{
	public abstract class RaStateBase<TParent> : MonoBehaviour, IRaState
	{
		public TParent Parent
		{
			get; private set;
		}

		public bool IsInitialized
		{
			get; private set;
		}

		public bool IsCurrentState
		{
			get; private set;
		}

		internal bool Init(TParent parent)
		{
			if(IsInitialized)
			{
				return false;
			}

			Parent = parent;
			IsInitialized = true;
			OnPreInitialize();
			OnInit();
			return true;
		}

		internal void PreSwitch()
		{
			OnPreSwitch();
		}

		internal virtual bool Enter()
		{
			if(IsCurrentState)
			{
				return false;
			}

			IsCurrentState = true;
			OnEnter();
			return true;
		}

		internal virtual bool Exit(bool isSwitch)
		{
			if(!IsCurrentState)
			{
				return false;
			}

			IsCurrentState = false;
			OnExit(isSwitch);
			return true;
		}

		internal bool Deinit()
		{
			if(!IsInitialized)
			{
				return false;
			}

			IsInitialized = false;
			OnDeinit();
			Parent = default;
			return true;
		}

		protected abstract void OnInit();
		protected abstract void OnEnter();
		protected abstract void OnExit(bool isSwitch);
		protected abstract void OnDeinit();

		protected virtual void OnPreInitialize()
		{

		}

		/// <summary>
		/// By checking <see cref="IsCurrentState"/>, you can determine whether to Prepare for an exit, or an enter 
		/// </summary>
		protected virtual void OnPreSwitch()
		{
		
		}
	}
}