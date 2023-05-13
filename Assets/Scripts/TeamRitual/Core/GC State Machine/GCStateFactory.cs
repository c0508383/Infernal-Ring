namespace TeamRitual.Core{
public class GCStateFactory {
    GCStateMachine stateMachine;
    public GCStateFactory(GCStateMachine stateMachine) {
        this.stateMachine = stateMachine;
    }

    public virtual GCState Intro() {
        return new GCStateIntro(stateMachine,this);
    }
    public virtual GCState Countdown() {
        return new GCStateCountdown(stateMachine,this);        
    }
    public virtual GCState Fight() {
        return new GCStateFight(stateMachine,this);        
    }
    public virtual GCState End() {
        return new GCStateEnd(stateMachine,this);        
    }
}
}