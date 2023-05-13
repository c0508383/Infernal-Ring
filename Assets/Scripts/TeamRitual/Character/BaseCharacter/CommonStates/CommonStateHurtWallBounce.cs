using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Core;

namespace TeamRitual.Character {
public class CommonStateHurtWallBounce : CharacterState
{
    public CommonStateHurtWallBounce(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CUSTOM;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.HURT;

        this.animationName = this.character.characterName + "_HurtWallBounce";
    }

    public override void EnterState() {
        base.EnterState();
        if (this.character.lastContact.WallBounce.y != 0 || this.character.lastContact.WallBounce.x != 0) {
            this.character.enemy.comboProcessor.AddWallBounce();
            EffectSpawner.PlayHitEffect(65, this.character.body.position, this.character.spriteRenderer.sortingOrder + 1, false);
            GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(65),false);
            this.character.VelY(this.character.lastContact.WallBounceSlide);
            if (this.character.lastContact.BounceGravity > 0) {
                this.character.gravity = this.character.lastContact.BounceGravity;
            }
        }
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.body.position.y <= 0.2 && this.character.VelY() < 0) {
            this.SwitchState(this.character.states.HurtSlide());
        } else if (this.character.lastContact.WallBounceTime == 0) {
            this.character.SetVelocity(this.character.lastContact.WallBounce);
            if (this.character.lastContact.WallBounceGravity != 0) {
                this.character.gravity = this.character.lastContact.WallBounceGravity;
            }
            this.SwitchState(this.character.states.HurtAir());
        } else {
            this.character.lastContact.WallBounceTime--;
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}