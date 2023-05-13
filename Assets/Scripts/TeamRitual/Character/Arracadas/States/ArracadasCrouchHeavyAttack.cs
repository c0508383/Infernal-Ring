namespace TeamRitual.Character {
public class ArracadasCrouchHeavyAttack : CharacterState
{
    public ArracadasCrouchHeavyAttack(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CUSTOM;
        this.moveType = MoveType.CROUCH;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.HEAVY;
        this.jumpCancel = true;
        this.hitsToCancel = 1;

        this.animationName = this.character.characterName + "_" + this.GetType().Name.Replace(this.character.characterName,"");
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.stateTime == 14) {
            this.character.VelX(this.character.VelX()*0.5f + 20);
        } else if (this.stateTime > 15) {
            this.character.VelXDirect(this.character.VelX()*0.9f);
        }

        if (this.character.anim.GetCurrentAnimatorStateInfo(0).IsName(this.animationName)
            && this.character.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            this.SwitchState(this.character.states.Crouch());
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}