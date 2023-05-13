using UnityEngine;

namespace TeamRitual.Character {
public class CommonStateStand : CharacterState
{
    public CommonStateStand(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = true;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = true;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.IDLE;

        this.animationName = this.character.characterName + "_Stand";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.VelY(0);
        this.character.body.position = new Vector2(this.character.PosX(),0);
    }

    public override void UpdateState() {
        base.UpdateState();
    }

    public override void ExitState() {
        base.ExitState();
    }
}
}