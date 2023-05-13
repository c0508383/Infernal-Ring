using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Core;
using UnityEngine;

namespace TeamRitual.Character{
public class ArracadasUltimateStart : CharacterState {
    int timeHit = 0;
    public ArracadasUltimateStart(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CUSTOM;
        this.moveType = MoveType.CROUCH;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SUPER;
        this.hitsToCancel = int.MaxValue;
        this.EXFlash = true;
        this.scalingStep = 0;

        this.animationName = "Arracadas_UltimateStart";
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.stateTime == 0) {
            this.character.VelX(0);
            this.character.VelY(0);
            GameController.Instance.Pause(30);
            GameController.Instance.playerPaused = this.character.playerNumber;
            GameController.Instance.CameraFocusCharacter(this.character);
            GameController.Instance.SetCameraZoom(4f);
            GameController.Instance.SetCameraLerp(30f);
            EffectSpawner.PlayHitEffect(1200, new Vector2(this.character.PosX(),this.character.PosY() + this.character.height/2f), this.character.spriteRenderer.sortingOrder + 1, true);
        }

        if (this.stateTime == 11) {
            GameController.Instance.ResetCameraLerp();
            GameController.Instance.CameraFocusReset();
            GameController.Instance.ResetCameraZoom();
        }

        if (this.moveHit > 0) {
            if (this.timeHit == 0) {
                this.timeHit = this.stateTime;
            } else if (this.stateTime - this.timeHit == 10) {
                this.SwitchState((this.states as ArracadasStateFactory).UltimatePunching());
            }
        }

        if (this.character.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            this.SwitchState(this.states.Crouch());
        }
    }
}
}