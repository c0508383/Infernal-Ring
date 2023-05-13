using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Core;

namespace TeamRitual.Character {
public class ArracadasFireLight : CharacterState
{
    public ArracadasFireLight(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SPECIAL;

        this.animationName = "Arracadas_FireLight";
    }

    public override void EnterState() {
        base.EnterState();
        (this.character as ArracadasStateMachine).UseBullet();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.stateTime == 5) {
            GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(2200), false);
        }

        if (GameController.Instance.AnimationOver(this.character.anim,this.animationName)) {
            this.SwitchState(this.states.Stand());
        }
    }
}
}