using UnityEngine;

namespace TeamRitual.Character{
public class XoninSpecial2HeavyChop : CharacterState {
    public XoninSpecial2HeavyChop(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
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

        this.animationName = this.character.characterName + "_Special2HeavyChop";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.VelY(-25);
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.character.body.position.y <= 0.2) {
            this.character.VelY(0);
            this.SwitchState((this.states as XoninStateFactory).Special2HeavyLand());
        }
    }    
}
}