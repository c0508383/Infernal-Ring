using BlackGardenStudios.HitboxStudioPro;
using UnityEngine;

namespace TeamRitual.Core
{
	public class GCStateCountdown : GCState
	{
		public GCStateCountdown(GCStateMachine stateMachine, GCStateFactory stateFactory)
			: base(stateMachine, stateFactory)
		{
			this.stateMachine = stateMachine;
			this.stateFactory = stateFactory;
		}

		public override void EnterState()
		{
			base.EnterState();
			if (GameController.Instance.currentRound <= 9) {
				EffectSpawner.PlayHitEffect(8010, (Vector3) new Vector2(0.0f, 0.0f), 20, false);
			}
		}

		public override void UpdateState()
		{
			base.UpdateState();
			if (this.stateTime <= 80)
				return;
			this.SwitchState(this.stateFactory.Fight());
		}
	}
}
