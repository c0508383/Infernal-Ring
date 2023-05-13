namespace TeamRitual.Character{
public class XoninSpecial2LightRise : CharacterState {
    public XoninSpecial2LightRise(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.AIR;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SPECIAL;

        this.animationName = this.character.characterName + "_Special2LightRise";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.SetVelocity(4,12);
    }

    public override void UpdateState() {
        base.UpdateState();
        this.character.SetVelocity(4,12);

        if (this.character.anim.GetCurrentAnimatorStateInfo(0).IsName(this.character.characterName + "_Special2LightRise")
            && this.character.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            this.SwitchState((this.states as XoninStateFactory).Special2LightChop());
        }
    }
}
}