using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Core;

namespace TeamRitual.Character {
public class ArracadasFireHeavy : CharacterState
{
    public ArracadasFireHeavy(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = true;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.STAND;
        this.moveType = MoveType.STAND;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SPECIAL;
        this.maxSelfChain = (this.character as ArracadasStateMachine).GetMaxBullets();
        this.EXFlash = true;

        this.animationName = "Arracadas_FireHeavy";
    }

    public override void EnterState() {
        base.EnterState();
        this.character.EXEffectStart();
    }

    public override void UpdateState() {
        base.UpdateState();

        if (this.stateTime == 5) {
            GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(2200), false);
        }

        if (this.moveHit == 1 && (this.character as ArracadasStateMachine).GetBullets() > 0) {
            (this.character as ArracadasStateMachine).UseBullet();
            this.SwitchState((this.states as ArracadasStateFactory).FireHeavy());
        }

        if (GameController.Instance.AnimationOver(this.character.anim,this.animationName)) {
            this.SwitchState(this.states.Stand());
        }
    }
}
}