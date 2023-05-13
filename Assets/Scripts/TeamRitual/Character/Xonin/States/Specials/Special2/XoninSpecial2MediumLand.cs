namespace TeamRitual.Character{
public class XoninSpecial2MediumLand : CharacterState {
    public XoninSpecial2MediumLand(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CROUCH;
        this.moveType = MoveType.CROUCH;
	    this.stateType = StateType.IDLE;

        this.attackPriority = AttackPriority.SPECIAL;

        this.animationName = this.character.characterName + "_Special2MediumLand";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.VelX(0);
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.anim.GetCurrentAnimatorStateInfo(0).IsName(this.character.characterName + "_Special2MediumLand")
            && this.character.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            this.SwitchState(this.states.CrouchTransition());
        }
    }
}
}