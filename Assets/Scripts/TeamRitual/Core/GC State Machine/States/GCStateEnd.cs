using BlackGardenStudios.HitboxStudioPro;
using TeamRitual.Character;
using TeamRitual.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TeamRitual.Core{
public class GCStateEnd : GCState {
    public GCStateEnd(GCStateMachine stateMachine, GCStateFactory stateFactory) : base(stateMachine,stateFactory) {
        this.stateMachine = stateMachine;
        this.stateFactory = stateFactory;
    }

    public override void EnterState() {
        GameController.Instance.Pause(40);
        PlayerGameObj player1 = GameController.Instance.Players[0];
        PlayerGameObj player2 = GameController.Instance.Players[1];
        PlayerGameObj playerGameObj = (PlayerGameObj) null;
        HUD_KO.KOType = KOType.NORMAL;
        if (GameController.Instance.remainingTimerTime == 0)
            HUD_KO.KOType = KOType.TIME;
        if ((double) player1.stateMachine.health > (double) player2.stateMachine.health)
        {
            ++player1.wins;
            playerGameObj = player1;
        }
        else if ((double) player1.stateMachine.health < (double) player2.stateMachine.health)
        {
            ++player2.wins;
            playerGameObj = player2;
        }
        else
        {
            ++player1.wins;
            ++player2.wins;
            if (HUD_KO.KOType != KOType.TIME)
                HUD_KO.KOType = KOType.DOUBLE;
        }
        EffectSpawner.PlayHitEffect(8000, (Vector3) new Vector2(0.0f, 0.0f), 2, false);
        string str = "normal";
        if ((Object) playerGameObj != (Object) null)
        {
            if ((double) playerGameObj.stateMachine.health == (double) playerGameObj.stateMachine.maxHealth)
                str = "perfect";
            else if (playerGameObj.stateMachine.lastContact.AttackPriority == AttackPriority.SUPER)
                str = "super";
            else if (GameController.Instance.remainingTimerTime == 0)
                str = "time";
        }
        else
            str = GameController.Instance.remainingTimerTime == 0 ? "time" : "draw";
        if ((Object) playerGameObj == (Object) player1)
        {
            Image p1WinImage = GameController.Instance.P1WinImages[player1.wins - 1];
            p1WinImage.sprite = UnityEngine.Resources.Load<Sprite>("Sprites/Versus/HUD/Win/P1win-" + str);
            p1WinImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else if ((Object) playerGameObj == (Object) player2)
        {
            Image p2WinImage = GameController.Instance.P2WinImages[player2.wins - 1];
            p2WinImage.sprite = UnityEngine.Resources.Load<Sprite>("Sprites/Versus/HUD/Win/P2win-" + str);
            p2WinImage.color = new Color(1f, 1f, 1f, 1f);
        }
        else
        {
            Image p1WinImage = GameController.Instance.P1WinImages[player1.wins - 1];
            p1WinImage.sprite = UnityEngine.Resources.Load<Sprite>("Sprites/Versus/HUD/Win/P1win-" + str);
            p1WinImage.color = new Color(1f, 1f, 1f, 1f);
            Image p2WinImage = GameController.Instance.P2WinImages[player2.wins - 1];
            p2WinImage.sprite = UnityEngine.Resources.Load<Sprite>("Sprites/Versus/HUD/Win/P2win-" + str);
            p2WinImage.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (this.stateTime > 40 && this.stateTime < 80 && this.stateTime % 2 == 0)
            GameController.Instance.Pause(1);
        PlayerGameObj player1 = GameController.Instance.Players[0];
        PlayerGameObj player2 = GameController.Instance.Players[1];
        if (player1.wins >= GameController.WinsNeeded || player2.wins >= GameController.WinsNeeded)
        {
            if (this.stateTime == 160) {
                string text = player1.wins == player2.wins ? "DRAW GAME" : player1.wins >= GameController.WinsNeeded ? player1.characterName + " WINS" : player2.characterName + " WINS";

                GameController.Instance.SetCameraZoom(4f);
                if (player1.wins >= GameController.WinsNeeded) {
                    GameController.Instance.CameraFocusCharacter(player1.stateMachine);
                } else if (player2.wins >= GameController.WinsNeeded) {
                    GameController.Instance.CameraFocusCharacter(player2.stateMachine);
                } else {
                    GameController.Instance.ResetCameraZoom();
                }
                WinText.ShowText(text);
            }
            if (this.stateTime == 370) {
                SceneManager.LoadScene("CharSelect", LoadSceneMode.Additive);
            }
        }
        else
        {
            CharacterStateMachine stateMachine1 = player1.stateMachine;
            CharacterStateMachine stateMachine2 = player2.stateMachine;
            if ((this.stateTime <= 220 || !(stateMachine1.currentState is CommonStateLyingDown) && !(stateMachine2.currentState is CommonStateLyingDown)) && (!(stateMachine1.currentState is CommonStateStand) && !(stateMachine2.currentState is CommonStateStand) || stateMachine1.currentState.stateTime <= 20 || stateMachine2.currentState.stateTime <= 20))
                return;
            if ((double) stateMachine1.DistanceToEnemy() < 5.0)
            {
                stateMachine1.currentState.SwitchState(stateMachine1.states.RunBack());
                stateMachine2.currentState.SwitchState(stateMachine2.states.RunBack());
            }
            this.SwitchState(this.stateFactory.Intro());
        }
    }
}
}