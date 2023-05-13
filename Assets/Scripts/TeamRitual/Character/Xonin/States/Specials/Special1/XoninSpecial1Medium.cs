namespace TeamRitual.Character{
public class XoninSpecial1Medium : CharacterState {
    public XoninSpecial1Medium(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CUSTOM;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SPECIAL;
        this.hitsToCancel = int.MaxValue;

        this.animationName = this.character.characterName + "_Special1Medium";
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.stateTime == 5) {
            this.physicsType = PhysicsType.AIR;
            this.character.SetVelocity(15,16);
        }

        if (this.character.anim.GetCurrentAnimatorStateInfo(0).IsName(this.character.characterName + "_Special1Medium")
            && this.character.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            this.animationName = this.character.characterName + "_Airborne";
            this.character.anim.Play(this.animationName);
        }
        
        if (this.character.PosY() <= 0 && this.character.VelY() < 0 && this.stateTime > 5) {
            this.character.VelY(0);
            this.SwitchState(this.character.states.JumpLand());
        }
    }
}
}