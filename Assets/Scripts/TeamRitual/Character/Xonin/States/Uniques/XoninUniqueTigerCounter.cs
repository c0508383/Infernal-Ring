using TeamRitual.Core;
using UnityEngine;

namespace TeamRitual.Character {
public class XoninUniqueTigerCounter : CharacterState
{
    public XoninUniqueTigerCounter(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SPECIAL;
        this.hitsToCancel = int.MaxValue;
        this.EXFlash = true;

        this.animationName = this.character.characterName + "_UniqueTigerCounter";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.VelX(50);
        this.character.EXEffectStart();
        this.MakeInvincible();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (GameController.Instance.AnimationOver(this.character.anim,this.animationName)) {
            GameController.Instance.ResetCameraZoom();
            this.SwitchState(this.character.states.CrouchTransition());
        }
    }
    
    public override bool OnHitEnemy() {
        if (this.moveHit == 3) {
            GameController.Instance.SetCameraZoom(4f);
        }
        return true;
	}

    public override bool OnHurt() {
        GameController.Instance.ResetCameraZoom();
        return true;
    }
}
}