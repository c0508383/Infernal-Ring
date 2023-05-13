
namespace TeamRitual.Character {
public class CommonStateAirborne : CharacterState
{
    public CommonStateAirborne(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory)
    : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = true;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = true;

        this.physicsType = PhysicsType.AIR;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_Airborne";
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void UpdateState() {
        base.UpdateState();

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