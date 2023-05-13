using BlackGardenStudios.HitboxStudioPro;
using UnityEngine;

namespace TeamRitual.Character {
public class CommonStateAirdashForward : CharacterState
{
    public CommonStateAirdashForward(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory)
    : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = true;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CUSTOM;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_RunForward";
    }

    public override void EnterState() {
        base.EnterState();

        EffectSpawner.PlayHitEffect(
            80, new Vector2(this.character.PosX(),this.character.PosY() + this.character.height/2), this.character.spriteRenderer.sortingOrder + 1, this.character.facing != 1
        );

        this.character.airdashCount++;
        this.character.SetVelocity(
            this.character.velocityAirdashForward.x * (this.character.GetRingMode() == RingMode.FIFTH ? 1.5f : 1.0f), this.character.velocityAirdashForward.y
        );
        this.character.SetVelocity(
            this.character.velocityAirdashForward.x * 
            (this.character.GetRingMode() == RingMode.FIFTH ? 1.5f : 1f),
            this.character.velocityAirdashForward.y
        );
    }

    public override void UpdateState() {
        base.UpdateState();

        this.character.SetVelocity(this.character.velocityAirdashForward);

        if (this.stateTime > 6) {
            this.SwitchState(this.character.states.Airborne());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}