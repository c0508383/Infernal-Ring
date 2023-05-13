using UnityEngine;

namespace TeamRitual.UI {
public class WinText : UIComponent {
    static WinText Instance;

    TMPro.TMP_Text winText;
    int flash;

    public override void Start() {
        base.Start();

        WinText.Instance = this;
        this.winText = this.GetComponent<TMPro.TMP_Text>();
        this.gameObject.SetActive(false);
    }

    public override void FixedUpdate() {
        base.FixedUpdate();

        if (flash > 0) {
            winText.color = Mathf.Ceil(ticks/20)%2 == 0 ? Color.white : Color.yellow;
            flash--;
        } else {
            winText.color = Color.white;
        }
    }

    public static void ShowText(string winText) {
        WinText.Instance.gameObject.SetActive(true);
        WinText.Instance.winText.text = winText;
        WinText.Instance.flash = 300;
    }
}
}