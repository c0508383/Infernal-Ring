using System;
using UnityEngine;

namespace TeamRitual.Character {
public class CommonStateJumpLand : CharacterState
{
    public CommonStateJumpLand(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory)
    : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = true;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_JumpLand";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.VelX(0);
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.stateTime >= 2) {
            this.character.body.MovePosition(
                new Vector2(this.character.body.position.x,(float)Math.Ceiling(this.character.body.position.y))
            );
            this.SwitchState(this.character.states.Stand());
        }
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}