using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamRitual.UI {
public class CharSelectCursor : UIComponent
{
    public int player = 1;

    bool selected = false;
    int slotSelected = 0;

    public CharSelectCursor() : base() {
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!selected) {
            //Quaternion target = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 200);
            //transform.rotation = Quaternion.Lerp(transform.rotation, target, 5*Time.deltaTime);
            float scale = 0.8f + (Mathf.Sin(ticks/20f)+1)/4f;
            Vector2 scaleVec = new Vector2(scale,scale);
            transform.localScale = scaleVec;
        } else {
            Quaternion target = Quaternion.Euler(0, 0, (player-1) * 45);
            transform.rotation = Quaternion.Lerp(transform.rotation, target, 5*Time.deltaTime);
            transform.localScale = new Vector2(1.0f,1.0f);
        }
    }

    public void LockSelection() {
        this.selected = true;
    }

    public bool SelectionLocked() {
        return this.selected;
    }

    public int GetSelectedSlot() {
        return this.slotSelected;
    }

    public int SelectForward(int amount) {
        if (!this.SelectionLocked()) {
            this.slotSelected++;
            this.slotSelected = this.slotSelected%CharSelectController.Instance.CharacterCount();
        }
        return this.slotSelected;
    }

    public int SelectBack(int amount) {
        if (!this.SelectionLocked()) {
            this.slotSelected--;
            if (this.slotSelected < 0) {
                this.slotSelected = CharSelectController.Instance.CharacterCount() + this.slotSelected;
            }
        }
        return this.slotSelected;
    }
}
}
