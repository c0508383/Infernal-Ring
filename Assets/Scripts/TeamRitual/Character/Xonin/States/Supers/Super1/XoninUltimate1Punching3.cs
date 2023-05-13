using BlackGardenStudios.HitboxStudioPro;
using UnityEngine;
using TeamRitual.Core;

namespace TeamRitual.Character{
public class XoninUltimate1Punching3 : CharacterState {
    public XoninUltimate1Punching3(CharacterStateMachine currentContext, CharacterStateFactory CharacterStateFactory) : base(currentContext, CharacterStateFactory)
    {
        this.inputChangeState = false;
        this.faceEnemyStart = false;
        this.faceEnemyAlways = false;

        this.physicsType = PhysicsType.CUSTOM;
        this.moveType = MoveType.AIR;
	    this.stateType = StateType.ATTACK;

        this.attackPriority = AttackPriority.SUPER;
        this.hitsToCancel = int.MaxValue;
        this.EXFlash = true;
        this.scalingStep = 0;

        this.animationName = this.character.characterName + "_Ultimate1Punching3";
    }

    public override void EnterState() {
        base.EnterState();
    }
    public override void UpdateState() {
        base.UpdateState();

        this.character.VelX(1f);
        this.character.VelY(0);

        if (this.stateTime > 150) {
            this.SwitchState((this.states as XoninStateFactory).Ultimate1PunchingEnd());
        }
    }

    public override bool OnHitEnemy() {
        if (this.stateTime%3 == 0) {
            float width = this.character.enemy.width;
            float randX = this.character.enemy.PosX() + Random.Range(-width*2f, width*2f);
            float randY = this.character.enemy.PosY() + Random.Range(0, width*5f);
            int sortingOrder = (int) Random.Range(-1,2);
            EffectSpawner.PlayHitEffect(/*this.stateTime%20 == 0 ? 402 : */Random.Range(0,10) >= 5 ? 300 : 400,
                new Vector2(randX,randY),
                this.character.spriteRenderer.sortingOrder + sortingOrder,
                true);
        }
        if (this.stateTime%12 == 0) {
            GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(71), true);
        } else if (this.stateTime%12 == 4)  {
            GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(71), false);
        } else if (this.stateTime%12 == 8)  {
            GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(71), false);
        }
        return true;
    }
}
}