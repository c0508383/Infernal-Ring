using TeamRitual.Core;
using UnityEngine;

namespace TeamRitual.Character {
public class XoninUniqueCrane : CharacterState
{
    public XoninUniqueCrane(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.IDLE;

        this.attackPriority = AttackPriority.SPECIAL;

        this.animationName = this.character.characterName + "_UniqueCrane";
    }

    public override void EnterState() {
        base.EnterState();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.stateTime == 6) {
            this.character.Flash(new Vector4(1.5f,1.5f,1.5f,1f),10);
        }

        if (GameController.Instance.AnimationOver(this.character.anim,this.animationName))
            this.SwitchState(this.states.Stand());
    }

    public override bool OnHurt() {
        if (this.character.enemy.currentState.moveType == MoveType.AIR && this.stateTime > 6 && this.stateTime <= 15) {
            this.SwitchState((this.states as XoninStateFactory).UniqueCraneCounter());
            return false;
        }
        return true;
    }
}
}