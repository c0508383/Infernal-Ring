using BlackGardenStudios.HitboxStudioPro;
using UnityEngine;

namespace TeamRitual.Character {
public class CommonStateAirdashBack : CharacterState
{
    public Vector2 jumpVelocity = Vector2.zero;

    public CommonStateAirdashBack(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory)
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

        this.character.airdashCount++;
        this.character.SetVelocity(
            this.character.velocityAirdashBack.x * 
            (this.character.GetRingMode() == RingMode.FIFTH ? 1.5f : 1f),
            this.character.velocityAirdashBack.y
        );
        this.character.Flash(new Vector4(1.5f,1.5f,1.5f,1f),7);
        this.MakeInvincible();

        EffectSpawner.PlayHitEffect(
            80, new Vector2(this.character.PosX(),this.character.PosY() + this.character.height/2), this.character.spriteRenderer.sortingOrder + 1, this.character.facing == 1
        );
    }

    public override void UpdateState() {
        base.UpdateState();

        this.character.SetVelocity(this.character.velocityAirdashBack);

        if (this.stateTime > 6) {
            this.ClearInvincibility();
            this.SwitchState(this.character.states.Airborne());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}