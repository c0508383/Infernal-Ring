using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Core;

namespace TeamRitual.Character {
public class ArracadasLoad : CharacterState
{
    public ArracadasLoad(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.IDLE;

        this.animationName = "Arracadas_Load";
    }

    public override void EnterState() {
        base.EnterState();

        //play load sound
        GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(2201), true);
        (this.character as ArracadasStateMachine).LoadBullet();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (GameController.Instance.AnimationOver(this.character.anim,this.animationName)) {
            this.SwitchState(this.states.Stand());
        }
    }
}
}