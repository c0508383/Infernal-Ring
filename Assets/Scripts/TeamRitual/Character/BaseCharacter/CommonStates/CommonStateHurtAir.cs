using TeamRitual.Core;
using UnityEngine;

namespace TeamRitual.Character {
public class CommonStateHurtAir : CharacterState
{
    public CommonStateHurtAir(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.AIR;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.HURT;

        this.animationName = this.character.characterName + "_HurtAir";
    }

    public override void EnterState() {
        base.EnterState();
        if (this.character.lastContact.HitFall && this.character.lastContact.FallingGravity > 0) {
            this.character.gravity = this.character.lastContact.FallingGravity;
        }
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.health > 0 && this.character.hitstun == 0) {
            if (this.character.lastContact.FallRecover &&
                (this.character.inputHandler.held("L") || this.character.inputHandler.held("M") || this.character.inputHandler.held("H"))) {
                this.SwitchState(this.character.states.Recover());
                return;
            } else if (!this.character.lastContact.HitFall) {
                this.SwitchState(this.character.states.Airborne());
                return;
            }
        }

        if (this.character.body.position.y <= 0 && this.character.VelY() < 0) {
            this.character.VelY(0);
            if (this.character.lastContact.HitFall || this.character.health == 0) {
                this.SwitchState(this.character.states.HurtBounce());
            } else {
                this.SwitchState(this.character.states.HurtStand());
            }
        } else if (this.character.lastContact.WallBounceTime > 0 && this.stateTime > 5) {
            float posX = this.character.PosX();

            float maxBound = GameController.Instance.StageMaxBound() - 0.05f - this.character.width/2f;
            float minBound = GameController.Instance.StageMinBound() + 0.05f + this.character.width/2f;

            if (posX >= maxBound || posX <= minBound) {
                this.character.VelX(0);
                this.SwitchState(this.character.states.HurtWallBounce());
            }
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}