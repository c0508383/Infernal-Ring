using UnityEngine;
using UnityEngine.InputSystem;

namespace TeamRitual.UI {
public class OptionsDevice : UIComponent
{
    public InputDevice device;
    public int slot;
    public int player;//0 - middle, 1 - P1, 2 - P2

    public int inputCooldown = 0;

    UIComponent leftArrow;
    UIComponent rightArrow;

    void Awake() {
        this.leftArrow = GameObject.Find("LeftArrow").GetComponent<UIComponent>();
        this.rightArrow = GameObject.Find("RightArrow").GetComponent<UIComponent>();
        this.leftArrow.positionLerp = 60f;
        this.rightArrow.positionLerp = 60f;
    }

    public override void FixedUpdate() {
        base.FixedUpdate();
        this.positionLerp = 20f;
        this.destX = player == 0 ? 0 : 425 * Mathf.Pow(-1, this.player%2);

        if (this.inputCooldown > 0) {
            this.inputCooldown--;
        }

        this.leftArrow.gameObject.SetActive(this.player != 1);
        this.rightArrow.gameObject.SetActive(this.player != 2);

        if (this.ticks%50 == 0) {
            this.BobArrow(this.leftArrow);
            this.BobArrow(this.rightArrow);
        }
    }

    public void BobArrow(UIComponent arrow) {
        int direction = arrow == this.leftArrow ? -1 : 1;
        float startX = 60 * direction;
        if (arrow.destX != startX) {
            arrow.destX = startX;
        } else {
            arrow.destX = startX + 10 * direction;
        }
    }

    public void SetPlayer(int player) {
        this.player = player;
        this.inputCooldown = 200;
    }
}
}