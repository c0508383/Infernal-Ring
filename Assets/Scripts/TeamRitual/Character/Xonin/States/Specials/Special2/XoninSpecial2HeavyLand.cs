using TeamRitual.Core;

namespace TeamRitual.Character{
public class XoninSpecial2HeavyLand : CharacterState {
    public XoninSpecial2HeavyLand(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CROUCH;
        this.moveType = MoveType.CROUCH;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SPECIAL;
        this.hitsToCancel = int.MaxValue;
        this.EXFlash = true;

        this.animationName = this.character.characterName + "_Special2HeavyLand";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.VelX(0);
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.anim.GetCurrentAnimatorStateInfo(0).IsName(this.character.characterName + "_Special2HeavyLand")
            && this.character.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            GameController.Instance.ResetCameraZoom();
            this.SwitchState(this.states.CrouchTransition());
        }
    }
    public override bool OnHitEnemy() {
        if (this.moveHit == 1) {
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