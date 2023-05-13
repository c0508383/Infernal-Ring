namespace TeamRitual.Character{
public class XoninSpecial1Light : CharacterState {
    public XoninSpecial1Light(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.AIR;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SPECIAL;
        this.hitsToCancel = int.MaxValue;

        this.animationName = this.character.characterName + "_Special1Light";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.SetVelocity(10,16);
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.anim.GetCurrentAnimatorStateInfo(0).IsName(this.character.characterName + "_Special1Light")
            && this.character.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            this.animationName = this.character.characterName + "_Airborne";
            this.character.anim.Play(this.animationName);
        }
        
        if (this.character.body.position.y <= 0 && this.character.VelY() < 0) {
            this.character.VelY(0);
            this.SwitchState(this.character.states.JumpLand());
        }
    }
}
}