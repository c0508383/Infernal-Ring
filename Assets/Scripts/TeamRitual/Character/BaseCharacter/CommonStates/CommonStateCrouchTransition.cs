namespace TeamRitual.Character {
public class CommonStateCrouchTransition : CharacterState
{
    bool standToCrouch = true;
    public CommonStateCrouchTransition(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory)
    : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = true;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = true;

        this.physicsType = PhysicsType.CROUCH;
        this.moveType = MoveType.CROUCH;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_CrouchTransition";
    }

    public override void EnterState() {
        base.EnterState();
        
        if(this.character.currentState == this.character.states.Crouch())
            standToCrouch = false;
    }

    public override void UpdateState() {
        base.UpdateState();

        if (!this.character.inputHandler.held("D")) {
            this.SwitchState(this.character.states.Stand());
        }

        if (this.stateTime >= 3) {
            if (standToCrouch) {
                this.SwitchState(this.character.states.Crouch());
            } else {
                this.SwitchState(this.character.states.Stand());
            }
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}