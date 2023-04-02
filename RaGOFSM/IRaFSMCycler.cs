namespace RaFSM
{
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