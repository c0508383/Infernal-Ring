namespace TeamRitual.Character {
public class ArracadasStandMediumAttack : CharacterState
{
    public ArracadasStandMediumAttack(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.MEDIUM;
        this.jumpCancel = true;
        this.hitsToCancel = 1;

        this.animationName = this.character.characterName + "_" + this.GetType().Name.Replace(this.character.characterName,"");
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void UpdateState() {
        base.UpdateState();

       if (this.character.anim.GetCurrentAnimatorStateInfo(0).IsName(this.animationName)
            && this.character.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
            this.SwitchState(this.character.states.Stand());
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}