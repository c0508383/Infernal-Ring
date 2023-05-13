namespace TeamRitual.Character {
public class CommonStateHurtCrouch : CharacterState
{
    public CommonStateHurtCrouch(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CROUCH;
        this.moveType = MoveType.CROUCH;
	    this.stateType = StateType.HURT;

        this.animationName = this.character.characterName + "_HurtCrouch";
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.PosY() > 0 || (this.character.health == 0 && this.character.hitstun == 0)) {
            this.SwitchState(this.character.states.HurtAir());
        } else if (this.character.hitstun == 0) {
            this.SwitchState(this.character.states.Crouch());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}