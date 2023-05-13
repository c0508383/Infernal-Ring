using UnityEngine;

namespace TeamRitual.Character{
public class XoninSpecial2MediumRise : CharacterState {
    public XoninSpecial2MediumRise(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CUSTOM;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SPECIAL;

        this.animationName = this.character.characterName + "_Special2MediumRise";
    }

    public override void EnterState() {
        base.EnterState();
    }
    public override void UpdateState() {
        base.UpdateState();

        if (this.stateTime < 5) {
            float yDiff = this.character.enemy.PosY() - this.character.PosY();
            if (yDiff > 0) {
                this.character.VelY(yDiff * 4 + 2);
            } else {
                this.character.VelY(8);
            }
        }

        if (Mathf.Abs(this.character.enemy.PosX() - this.character.PosX()) > 0.5f) {
            this.character.VelX(Mathf.Pow(Mathf.Abs(this.character.enemy.PosX() - this.character.PosX()),1.5f));
        }

        if (this.character.anim.GetCurrentAnimatorStateInfo(0).IsName(this.character.characterName + "_Special2MediumRise")
            && this.character.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            this.SwitchState((this.states as XoninStateFactory).Special2MediumChop());
        }
    }
}
}