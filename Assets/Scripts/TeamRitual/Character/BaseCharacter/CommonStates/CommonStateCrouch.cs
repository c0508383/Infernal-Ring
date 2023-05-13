
namespace TeamRitual.Character {
public class CommonStateCrouch : CharacterState
{
    public CommonStateCrouch(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory)
    : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = true;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = true;

        this.physicsType = PhysicsType.CROUCH;
        this.moveType = MoveType.CROUCH;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_Crouch";
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (!this.character.inputHandler.held("D")) {
            this.SwitchState(this.character.states.CrouchTransition());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}