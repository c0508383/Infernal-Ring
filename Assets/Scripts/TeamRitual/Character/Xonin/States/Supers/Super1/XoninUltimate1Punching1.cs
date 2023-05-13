using UnityEngine;

namespace TeamRitual.Character{
public class XoninUltimate1Punching1 : CharacterState {
    public XoninUltimate1Punching1(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CUSTOM;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SUPER;
        this.hitsToCancel = int.MaxValue;
        this.scalingStep = 0;

        this.animationName = this.character.characterName + "_Ultimate1Punching1";
    }

    public override void EnterState() {
        base.EnterState();
    }
    public override void UpdateState() {
        base.UpdateState();

        this.character.VelX(0.2f);
        this.character.VelY(0);

        if (this.stateTime > 50) {
            this.SwitchState((this.states as XoninStateFactory).Ultimate1Punching2());
        }
    }
}
}