namespace TeamRitual.Core{
public class GCState {
    public GCStateMachine stateMachine;
    public GCStateFactory stateFactory;

    public int stateTime = 0;

    public GCState(GCStateMachine stateMachine, GCStateFactory stateFactory) {
        this.stateMachine = stateMachine;
        this.stateFactory = stateFactory;
    }

    public virtual void EnterState() {
	}

	public virtual void UpdateState() {
		this.stateTime++;
	}

	public virtual void ExitState() {}

	public virtual void CheckSwitchState() {}

	public virtual void InitializeSubState() {}

	public virtual void SwitchState(GCState newState)
	{
		if (this.GetType() == newState.GetType())
			return;
        
		// exit current state
		ExitState();

		//enter new state
		newState.EnterState();

		//update context of state
		stateMachine.currentState = newState;

		//Debug.Log("Switched from " + this + " to " + newState);
	}

	protected void SetSuperState()
	{

	}

	protected void SetSubState()
	{

	}
}
}