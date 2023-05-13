using TeamRitual.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TeamRitual.UI {
public class ComboCounter : UIComponent {    
    public Image image;
    public TMPro.TMP_Text text;
    int shake;
    int prevHits;

    void Awake() {
        this.positionLerp = 50f;
    }

    void Update() {
        if (shake > 0) {
            this.shake--;
            this.destX = this.originalPosition.x + UnityEngine.Random.Range(-1,1) * 50;
            this.destY = this.originalPosition.y + UnityEngine.Random.Range(-1,1) * 50;
        } else {
            this.destX = this.originalPosition.x;
            this.destY = this.originalPosition.y;
        }
        
        if (this.prevHits >= 50 && this.text != null) {
            this.text.color = UnityEngine.Random.Range(0,100) > 50 ? new Color(1,0,0.8f,255) : new Color(0.75f,0,0,255);
        }
    }

    public void SetHits(int hits, RingMode ringMode) {
        if (hits != this.prevHits && this.text != null) {
            image.sprite = GameController.RingImages[hits%10];
            image.color = GameController.Instance.GetRingColor(ringMode);
            this.gameObject.SetActive(true);
            text.text = hits + " HITS!";
            this.shake = 20;
            this.prevHits = hits;

            if (hits >= 99) {
                text.text = (UnityEngine.Random.Range(0,100) > 50 ? "HITS: UNCALCULABLE" : "LOST COUNT...")
                    + (UnityEngine.Random.Range(0,100) > 50 ? "\nWTF!!?!?!!" : "\n!!!!!!STOP!!!!!!");
            } else if (hits >= 50) {
                text.text += "\n!!!INFERNAL!!!";
            } else if (hits >= 20) {
                text.text += UnityEngine.Random.Range(0,100) > 50 ? "\nAWESOME!!" : "\nMARVELOUS!!";
                text.color = new Color(1.00f, 0.38f, 0.49f, 255);
            } else if (hits >= 10) {
                text.text += UnityEngine.Random.Range(0,100) > 50 ? "\nNICE!" : "\nGREAT!";
                text.color = new Color(1.00f, 0.81f, 0.81f, 255);
            }
        }
    }

    public void ClearHits() {
        this.prevHits = 0;
        if (text != null) {
            text.text = "";
            text.color = new Color(1,1,1,255);
        }
        this.gameObject.SetActive(false);
    }
}
}