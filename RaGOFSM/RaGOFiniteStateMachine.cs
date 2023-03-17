using System.Collections.Generic;
using UnityEngine;

namespace RaFSM
{
	public class RaGOFiniteStateMachine : RaFiniteStateMachine<Component>
	{
		public RaGOFiniteStateMachine(Component parent, RaGOStateBase[] states, bool excludeDisabledStates = true)
			: base(parent, states, excludeDisabledStates)
		{
		}

		public static RaGOStateBase[] GetGOStates(Transform transform)
		{
			List<RaGOStateBase> states = new List<RaGOStateBase>();
			for(int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if(child.TryGetComponent(out RaGOStateBase state))
				{
					states.Add(state);
				}
			}
			return states.ToArray();
		}
	}
}