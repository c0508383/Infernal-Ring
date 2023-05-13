namespace TeamRitual.Character {
public class CommonStateWalkBackward : CharacterState
{
    public CommonStateWalkBackward(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = true;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_WalkBackward";
    }

    public override void EnterState() {
        base.EnterState();

        this.character.SetVelocity(this.character.velocityWalkBack);
    }

    public override void UpdateState() {
        base.UpdateState();
        
        this.character.SetVelocity(this.character.velocityWalkBack);

        if (!this.character.inputHandler.held(this.character.inputHandler.BackInput(this.character))) {
            this.SwitchState(this.character.states.Stand());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}