using UnityEngine;

namespace TeamRitual
{
    public class RingSelect : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;

        float scale = 2.5f;
        float ticks = 0;
        float maxTicks = 200;

        void Start()
        {
            this.spriteRenderer = GetComponent<SpriteRenderer>();
            this.transform.localScale = new Vector2(0,0);
        }

        void FixedUpdate()
        {
            if (ticks < maxTicks) {
                this.transform.localRotation = Quaternion.Euler(0f, 0f, ticks*2);
                this.transform.localScale = new Vector2(scale * (1 - ticks/maxTicks),scale * (1 - ticks/maxTicks));
                this.spriteRenderer.color = new Color(this.spriteRenderer.color.r,this.spriteRenderer.color.g,this.spriteRenderer.color.b,
                    1f - (float)ticks/(float)maxTicks);
                this.ticks++;
            } else {
                this.spriteRenderer.color = new Color(1f,1f,1f,0);
                Destroy(this);
            }
        }
    }
}
