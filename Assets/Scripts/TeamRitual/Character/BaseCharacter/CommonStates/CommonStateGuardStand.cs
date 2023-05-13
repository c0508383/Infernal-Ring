namespace TeamRitual.Character {
public class CommonStateGuardStand : CharacterState {
    public CommonStateGuardStand(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_GuardStand";
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.PosY() > 0) {
            this.SwitchState(this.character.states.GuardAir());
        } else if (this.character.blockstun == 0) {
            this.SwitchState(this.character.states.Stand());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}