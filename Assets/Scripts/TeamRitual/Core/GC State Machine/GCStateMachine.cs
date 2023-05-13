namespace TeamRitual.Core{
public class GCStateMachine {
    public GCState currentState;
    public GCStateFactory states;
    public GameController gameController;

    public GCStateMachine(GameController gameController) {
        this.states = new GCStateFactory(this);
        this.gameController = gameController;
    }
}
}