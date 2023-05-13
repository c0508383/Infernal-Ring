using TeamRitual.Core;

namespace TeamRitual.Character {
public class XoninUniqueDragon : CharacterState
{
    public XoninUniqueDragon(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.IDLE;

        this.attackPriority = AttackPriority.SPECIAL;
        this.EXFlash = true;

        this.animationName = this.character.characterName + "_UniqueDragon";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.EXEffectStart();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (GameController.Instance.AnimationOver(this.character.anim,this.animationName))
            this.SwitchState(this.character.states.Stand());
    }

    public override bool OnHurt() {
        CharacterState counterState = (this.states as XoninStateFactory).Special2HeavyRise();
        counterState.MakeInvincible();
        this.SwitchState(counterState);
        return false;
    }
}
}