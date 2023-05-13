using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Core;
using UnityEngine;

namespace TeamRitual.Character {
public class CommonStateHurtBounce : CharacterState
{
    public CommonStateHurtBounce(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.AIR;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.HURT;

        this.animationName = this.character.characterName + "_HurtBounce";
    }

    public override void EnterState() {
        base.EnterState();
        if (this.character.lastContact.Bounce.y != 0 || this.character.lastContact.Bounce.x != 0) {
            this.character.enemy.comboProcessor.AddGroundBounce();
            EffectSpawner.PlayHitEffect(60, this.character.body.position, this.character.spriteRenderer.sortingOrder + 1, true);
            GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(60),false);
            this.character.SetVelocity(this.character.lastContact.Bounce);
            if (this.character.lastContact.BounceGravity > 0) {
                this.character.gravity = this.character.lastContact.BounceGravity;
            }
        }
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.VelY() <= 0 && this.character.lastContact.BounceRecover &&
            (this.character.inputHandler.held("L") || this.character.inputHandler.held("M") || this.character.inputHandler.held("H"))) {
            this.SwitchState(this.character.states.Recover());
        }

        if (this.stateTime > 5 && this.character.body.position.y <= -0.1f && this.character.VelY() < 0) {
            this.SwitchState(this.character.states.HurtSlide());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}