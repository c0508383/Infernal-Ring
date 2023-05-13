using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Core;
using UnityEngine;

namespace TeamRitual.Character {
public class CommonStateLyingDown : CharacterState
{
    public CommonStateLyingDown(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CROUCH;
        this.moveType = MoveType.LYING;
	    this.stateType = StateType.HURT;

        this.animationName = this.character.characterName + "_LyingDown";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.VelY(0);
        this.character.body.position = new Vector2(this.character.PosX(),0);
        GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(40),false);
        EffectSpawner.PlayHitEffect(
            61, new Vector2(this.character.PosX(),this.character.PosY()), this.character.spriteRenderer.sortingOrder + 1, this.character.facing != 1
        );
    }

    public override void UpdateState() {
        base.UpdateState();

        int downTime = this.character.lastContact.DownTime > 0 ? this.character.lastContact.DownTime : 30;
        
        if (this.character.lastContact.DownRecover && this.character.inputHandler.held("D")) {
            downTime += 20;
        }

        if (stateTime > downTime && this.character.health > 0) {
            if (this.character.lastContact.DownRecover &&
                (this.character.inputHandler.held("L") || this.character.inputHandler.held("M") || this.character.inputHandler.held("H"))) {
                this.SwitchState(this.character.states.Recover());
            } else {
                this.SwitchState(this.character.states.JumpLand());
            }
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}