using TeamRitual.Core;

namespace TeamRitual.Character {
public class XoninSpecial1Heavy : CharacterState {
    public XoninSpecial1Heavy(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.AIR;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SPECIAL;
        this.hitsToCancel = int.MaxValue;
        this.EXFlash = true;

        this.animationName = this.character.characterName + "_Special1Heavy";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.SetVelocity(20,16);
        this.character.EXEffectStart();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.anim.GetCurrentAnimatorStateInfo(0).IsName(this.character.characterName + "_Special1Heavy")) {
            if (this.character.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
                GameController.Instance.ResetCameraZoom();
                this.SwitchState(this.states.Airborne());
            }
        }
        
        if (this.character.body.position.y <= 0.2 && this.character.VelY() < 0) {
            GameController.Instance.ResetCameraZoom();
            this.character.VelY(0);
            this.SwitchState(this.states.JumpLand());
        }
    }
    
    public override bool OnHitEnemy() {
        if (this.moveHit == 5) {
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