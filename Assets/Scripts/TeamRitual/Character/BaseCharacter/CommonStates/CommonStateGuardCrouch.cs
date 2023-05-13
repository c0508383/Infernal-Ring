namespace TeamRitual.Character {
public class CommonStateGuardCrouch : CharacterState {
    public CommonStateGuardCrouch(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CROUCH;
        this.moveType = MoveType.CROUCH;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_GuardCrouch";
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.PosY() > 0) {
            this.SwitchState(this.character.states.GuardAir());
        } else if (this.character.blockstun == 0) {
            this.SwitchState(this.character.states.Crouch());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}