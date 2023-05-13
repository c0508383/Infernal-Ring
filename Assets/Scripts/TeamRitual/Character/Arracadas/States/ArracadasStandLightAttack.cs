namespace TeamRitual.Character {
public class ArracadasStandLightAttack : CharacterState
{
    public ArracadasStandLightAttack(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.LIGHT;

        this.animationName = this.character.characterName + "_StandLightAttack";
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