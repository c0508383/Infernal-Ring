namespace TeamRitual.Character{
public class XoninSpecial2MediumChop : CharacterState {
    public XoninSpecial2MediumChop(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.AIR;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SPECIAL;
        this.hitsToCancel = int.MaxValue;

        this.animationName = this.character.characterName + "_Special2MediumChop";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.VelY(-12);
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.body.position.y <= 0 && this.character.VelY() < 0) {
            this.character.VelY(0);
            this.SwitchState((this.states as XoninStateFactory).Special2MediumLand());
        }
    }    
}
}