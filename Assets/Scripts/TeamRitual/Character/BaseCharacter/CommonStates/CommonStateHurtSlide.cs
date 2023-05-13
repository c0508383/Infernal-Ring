using TeamRitual.Core;
using BlackGardenStudios.HitboxStudioPro;

namespace TeamRitual.Character {
public class CommonStateHurtSlide : CharacterState
{
    public CommonStateHurtSlide(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CUSTOM;
        this.moveType = MoveType.LYING;
	    this.stateType = StateType.HURT;

        this.animationName = this.character.characterName + "_HurtBounce";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.SetVelocity(this.character.lastContact.Slide,0);
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.lastContact.SlideTime > 0) {
            if (this.character.lastContact.SlideTime%6 == 0) {
                EffectSpawner.PlayHitEffect(71, this.character.body.position, this.character.spriteRenderer.sortingOrder + 1, this.character.facing == -1);
            }
            if (this.character.lastContact.SlideTime%12 == 0) {
                GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(50),false);
            }
            this.character.lastContact.SlideTime--;
        } else {
            this.character.SetVelocity(0,0);
            this.SwitchState(this.character.states.LyingDown());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}