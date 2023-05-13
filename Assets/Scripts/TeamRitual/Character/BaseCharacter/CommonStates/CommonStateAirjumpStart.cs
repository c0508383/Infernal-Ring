using UnityEngine;

namespace TeamRitual.Character {
public class CommonStateAirjumpStart : CharacterState
{
    public Vector2 jumpVelocity = Vector2.zero;

    public CommonStateAirjumpStart(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory)
    : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.AIR;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_Airjump";
    }

    public override void EnterState() {
        base.EnterState();

        this.character.airjumpCount++;
        if (this.jumpVelocity == Vector2.zero) {
            this.jumpVelocity = this.character.velocityJumpNeutral;
        }
        AdjustVelocity();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.stateTime == 4) {
            this.SwitchState(this.character.states.Airborne());
            this.character.SetVelocity(new Vector2(jumpVelocity.x,jumpVelocity.y/1.1f));
        } else {
            AdjustVelocity();
        }
    }

    public override void ExitState() {
        base.ExitState();
    }

    public void AdjustVelocity() {
        if (jumpVelocity == this.character.velocityJumpNeutral) {
            if (this.character.inputHandler.held(this.character.inputHandler.ForwardInput(this.character)) ||
                this.character.GetInput().EndsWith("U,F") || this.character.GetInput().EndsWith("F,U")) {
                jumpVelocity = this.character.velocityJumpForward * 1.1f;
            } else if (this.character.inputHandler.held(this.character.inputHandler.BackInput(this.character)) ||
                this.character.GetInput().EndsWith("U,B") || this.character.GetInput().EndsWith("B,U")) {
                jumpVelocity = this.character.velocityJumpBack * 1.1f;
            }
        }
    }
}
}