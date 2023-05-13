namespace TeamRitual.Character {
public class CommonStateWalkForward : CharacterState
{
    public CommonStateWalkForward(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = true;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_WalkForward";
    }

    public override void EnterState() {
        base.EnterState();

        this.character.SetVelocity(this.character.velocityWalkForward);
    }

    public override void UpdateState() {
        base.UpdateState();
        
        this.character.SetVelocity(this.character.velocityWalkForward);

        if (!this.character.inputHandler.held(this.character.inputHandler.ForwardInput(this.character))) {
            this.SwitchState(this.character.states.Stand());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}