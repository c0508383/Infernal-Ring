using BlackGardenStudios.HitboxStudioPro;
using UnityEngine;

namespace TeamRitual.Character {
public class CommonStateRunForward : CharacterState
{
    public CommonStateRunForward(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory)
    : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = true;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_RunForward";
    }

    public override void EnterState() {
        base.EnterState();
        EffectSpawner.PlayHitEffect(
            90, new Vector2(this.character.PosX()+1.5f*this.character.facing,this.character.PosY()), this.character.spriteRenderer.sortingOrder + 1, this.character.facing != 1
        );
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.stateTime%10 == 0) {
            EffectSpawner.PlayHitEffect(
                70, new Vector2(this.character.PosX(),this.character.PosY()), this.character.spriteRenderer.sortingOrder + 1, this.character.facing != 1
            );
        }
        this.character.SetVelocity(
            this.character.velocityRunForward.x * 
            (this.character.GetRingMode() == RingMode.FIFTH ? 1.5f : this.character.GetRingMode() == RingMode.EIGHTH ? 0.7f : 1f),
            this.character.velocityRunForward.y
        );

        if (!this.character.inputHandler.held(this.character.inputHandler.ForwardInput(this.character)) && this.stateTime > 4) {
            this.SwitchState(this.character.states.Stand());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}