
using UnityEngine;

namespace TeamRitual.Character {
public class CommonStateRecover : CharacterState
{
    public CommonStateRecover(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory)
    : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = true;

        this.physicsType = PhysicsType.CUSTOM;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_Airborne";
    }

    public override void EnterState() {
        base.EnterState();

        Vector2 recoverVelocity = new Vector2(0,10);
        if (this.character.inputHandler.held(this.character.inputHandler.BackInput(this.character))) {
            recoverVelocity = new Vector2(-5,8);
        } else if (this.character.inputHandler.held(this.character.inputHandler.ForwardInput(this.character))) {
            recoverVelocity = new Vector2(5,8);
        }

        this.character.SetVelocity(recoverVelocity);
        this.MakeInvincible();
        this.character.Flash(new Vector4(20f,20f,20f,1f),6);
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.stateTime > 6) {
            this.ClearInvincibility();
            if (this.character.body.position.y <= 0.2 && this.character.VelY() < 0) {
                this.character.VelY(0);
                this.SwitchState(this.character.states.JumpLand());
            } else {
                this.SwitchState(this.character.states.Airborne());
            }
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}