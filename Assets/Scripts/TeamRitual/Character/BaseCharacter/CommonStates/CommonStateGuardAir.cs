namespace TeamRitual.Character {
public class CommonStateGuardAir : CharacterState {
    public CommonStateGuardAir(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.AIR;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_GuardAir";
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.PosY() <= 0) {
            this.SwitchState(this.character.states.GuardStand());
        } else if (this.character.blockstun == 0) {
            this.SwitchState(this.character.states.Airborne());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}