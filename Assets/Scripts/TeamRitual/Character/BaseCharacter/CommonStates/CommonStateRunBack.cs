using BlackGardenStudios.HitboxStudioPro;
using UnityEngine;

namespace TeamRitual.Character {
public class CommonStateRunBack : CharacterState
{
    public CommonStateRunBack(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory)
    : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = true;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_RunBack";
    }

    public override void EnterState() {
        base.EnterState();

        this.character.SetVelocity(this.character.velocityRunBack);
        EffectSpawner.PlayHitEffect(
            91, new Vector2(this.character.PosX() + 1.5f * this.character.facing,this.character.PosY()), this.character.spriteRenderer.sortingOrder + 1, this.character.facing != 1
        );
    }

    public override void UpdateState() {
        base.UpdateState();

        this.character.SetVelocity(this.character.velocityRunBack);

        if (this.stateTime == 3) {
            this.character.Flash(new Vector4(1.5f,1.5f,1.5f,1f),6);
            this.MakeInvincible();
        }
        if (this.stateTime == 9) {
            this.ClearInvincibility();
        }

        if (!this.character.inputHandler.held(this.character.inputHandler.BackInput(this.character)) && this.stateTime > 4) {
            this.ClearInvincibility();
            this.SwitchState(this.character.states.Stand());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}