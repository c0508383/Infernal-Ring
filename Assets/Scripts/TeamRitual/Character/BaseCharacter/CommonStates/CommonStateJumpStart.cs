using UnityEngine;

namespace TeamRitual.Character {
public class CommonStateJumpStart : CharacterState
{
    public Vector2 jumpVelocity = Vector2.zero;

    public CommonStateJumpStart(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory)
    : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_JumpStart";
    }

    public override void EnterState() {
        base.EnterState();

        if (this.jumpVelocity == Vector2.zero) {
            this.jumpVelocity = this.character.velocityJumpNeutral;
        }
        AdjustVelocity();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.stateTime == 5) {
            this.SwitchState(this.character.states.Airborne());
        } else {
            AdjustVelocity();
        }
    }

    public override void ExitState() {
        base.ExitState();
        
        this.character.SetVelocity(this.jumpVelocity);
        this.character.SetVelocity(
            this.jumpVelocity.x * 
            (this.character.GetRingMode() == RingMode.FIFTH ? 1.5f : this.character.GetRingMode() == RingMode.EIGHTH ? 0.7f : 1f),
            this.jumpVelocity.y
        );
    }

    void AdjustVelocity() {
        if (jumpVelocity == this.character.velocityJumpNeutral) {
            if (this.character.inputHandler.held(this.character.inputHandler.ForwardInput(this.character))) {
                jumpVelocity = this.character.velocityJumpForward;
            } else if (this.character.inputHandler.held(this.character.inputHandler.BackInput(this.character))) {
                jumpVelocity = this.character.velocityJumpBack;
            }

            float prevVelX = this.character.VelX()/this.character.standingFriction;
            if (Mathf.Sign(prevVelX * this.character.facing) == Mathf.Sign(jumpVelocity.x) && Mathf.Abs(prevVelX) > Mathf.Abs(jumpVelocity.x)) {
                jumpVelocity = new Vector2(jumpVelocity.x * 2f, jumpVelocity.y);
            }
        }
    }
}
}