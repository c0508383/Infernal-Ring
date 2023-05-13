namespace TeamRitual.Core{
public class GCStateIntro : GCState {
    public GCStateIntro(GCStateMachine stateMachine, GCStateFactory stateFactory) : base(stateMachine,stateFactory) {
        this.stateMachine = stateMachine;
        this.stateFactory = stateFactory;
    }

    public override void EnterState() {
        base.EnterState();

        GameController.Instance.remainingTimerTime = GameController.MaxTimerTime;
    }

    public override void UpdateState() {
        base.UpdateState();

        PlayerGameObj P1 = GameController.Instance.Players[0];
        PlayerGameObj P2 = GameController.Instance.Players[1];
        if (stateTime > 80 && P1.paletteSelected && P2.paletteSelected && P1.modeSelected && P2.modeSelected &&
            P1.stateMachine.PosY() == 0 && P2.stateMachine.PosY() == 0) {
            GameController.Instance.CameraFocusReset();
            this.SwitchState(this.stateFactory.Countdown());
        }
    }
}
}