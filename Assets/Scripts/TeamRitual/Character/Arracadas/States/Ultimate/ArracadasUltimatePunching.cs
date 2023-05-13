using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Core;
using UnityEngine;

namespace TeamRitual.Character{
public class ArracadasUltimatePunching : CharacterState {
    public ArracadasUltimatePunching(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CUSTOM;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SUPER;
        this.hitsToCancel = int.MaxValue;
        this.EXFlash = true;
        this.scalingStep = 0;

        this.animationName = "Arracadas_UltimatePunching";
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (moveHit == 1) {
            GameController.Instance.CameraFocusCharacter(this.character.enemy);
            GameController.Instance.SetCameraZoom(4f);
            GameController.Instance.SetCameraLerp(30f);
        }

        if (this.character.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            this.SwitchState(this.states.Stand());
            GameController.Instance.ResetCameraLerp();
            GameController.Instance.CameraFocusReset();
            GameController.Instance.ResetCameraZoom();
        }
    }
}
}