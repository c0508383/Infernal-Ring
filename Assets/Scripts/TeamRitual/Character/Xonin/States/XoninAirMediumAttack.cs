namespace TeamRitual.Character {
public class XoninAirMediumAttack : CharacterState
{
    public XoninAirMediumAttack(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.AIR;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.MEDIUM;

        this.animationName = this.character.characterName + "_AirMediumAttack";
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.anim.GetCurrentAnimatorStateInfo(0).IsName(this.animationName)
            && this.character.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            this.SwitchState(this.character.states.Airborne());

        if (this.character.body.position.y <= 0.2 && this.character.VelY() < 0) {
            this.character.VelY(0);
            this.SwitchState(this.character.states.JumpLand());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}