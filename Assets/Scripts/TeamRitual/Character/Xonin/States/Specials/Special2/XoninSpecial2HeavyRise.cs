using UnityEngine;

namespace TeamRitual.Character{
public class XoninSpecial2HeavyRise : CharacterState {
    public XoninSpecial2HeavyRise(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CUSTOM;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SPECIAL;
        this.EXFlash = true;

        this.animationName = this.character.characterName + "_Special2HeavyRise";
    }

    public override void EnterState() {
        base.EnterState();

        this.character.PosX(this.character.enemy.PosX() - 1 * this.character.facing);
        this.character.PosY(this.character.enemy.PosY() + 4);
        this.character.VelX(0);
        this.character.EXEffectStart();
    }
    public override void UpdateState() {
        base.UpdateState();

        if (this.character.anim.GetCurrentAnimatorStateInfo(0).IsName(this.character.characterName + "_Special2HeavyRise")
            && this.character.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            this.SwitchState((this.states as XoninStateFactory).Special2HeavyChop());
        }
    }
}
}