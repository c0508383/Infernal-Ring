using TeamRitual.Core;
using UnityEngine;

namespace TeamRitual
{
    public class RingSwitch : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        Animator animator;

        float scale = 3.0f;
        string animationName = "ringswitch";
        int ticks = 0;
        int maxTicks = 60;

        void Start()
        {
            this.spriteRenderer = GetComponent<SpriteRenderer>();
            this.transform.localScale = new Vector2(this.scale,this.scale);
            this.animator = GetComponent<Animator>();
            this.animator.Play(this.animationName);
        }

        void FixedUpdate()
        {
            if (ticks < maxTicks) {
                //this.spriteRenderer.color = new Color(1f,1f,1f,1f - (float)ticks/(float)maxTicks);
                this.ticks++;
            }

            if (GameController.Instance.AnimationOver(this.animator,this.animationName)) {
                this.spriteRenderer.color = new Color(1f,1f,1f,0);
                Destroy(this);
            }
        }
    }
}
