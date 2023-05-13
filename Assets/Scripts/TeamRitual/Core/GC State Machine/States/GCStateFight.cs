using BlackGardenStudios.HitboxStudioPro;
using UnityEngine;

namespace TeamRitual.Core{
public class GCStateFight : GCState {
    public GCStateFight(GCStateMachine stateMachine, GCStateFactory stateFactory) : base(stateMachine,stateFactory) {
        this.stateMachine = stateMachine;
        this.stateFactory = stateFactory;
    }

    public override void EnterState() {
        base.EnterState();
        EffectSpawner.PlayHitEffect(8020, (Vector3) new Vector2(0.0f, 0.0f), 20, false);
    }

    public override void UpdateState() {
        base.UpdateState();

        PlayerGameObj P1 = GameController.Instance.Players[0];
        PlayerGameObj P2 = GameController.Instance.Players[1];
        if (GameController.Instance.remainingTimerTime == 0 || P1.stateMachine.health == 0 || P2.stateMachine.health == 0) {
            this.SwitchState(this.stateFactory.End());
        }
    }
}
}