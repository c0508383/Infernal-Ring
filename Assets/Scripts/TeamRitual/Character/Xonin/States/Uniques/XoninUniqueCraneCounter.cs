using TeamRitual.Core;

namespace TeamRitual.Character {
public class XoninUniqueCraneCounter : CharacterState
{
    public XoninUniqueCraneCounter(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.AIR;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SPECIAL;
        this.hitsToCancel = int.MaxValue;
        this.EXFlash = true;

        this.animationName = this.character.characterName + "_UniqueCraneCounter";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.VelX(8);
        this.character.VelY(20);
        this.character.EXEffectStart();
        this.MakeInvincible();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (GameController.Instance.AnimationOver(this.character.anim,this.animationName) && this.character.VelY() < -1) {
            GameController.Instance.ResetCameraZoom();
            this.SwitchState(this.character.states.Airborne());
        }
    }
    
    public override bool OnHitEnemy() {
        if (this.moveHit == 6) {
            GameController.Instance.Pause(20);
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