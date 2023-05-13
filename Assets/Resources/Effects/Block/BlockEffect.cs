using UnityEngine;

namespace TeamRitual
{
    public class BlockEffect : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        float scale = 0.6f;
        float updateTicks;
        float maxTime;
        // Start is called before the first frame update
        void Start()
        {
            updateTicks = 0;
            maxTime = 15*10;
            spriteRenderer = GetComponent<SpriteRenderer>();
            transform.localScale = new Vector2(scale,scale);
        }

        void FixedUpdate()
        {
            updateTicks++;

            if (this.updateTicks > 5) {
                transform.localScale = new Vector2(scale*(1 + 0.5f*(updateTicks/maxTime)), scale*(1 + 0.5f*(updateTicks/maxTime)));
            }

            if (this.updateTicks > 10) {
                spriteRenderer.color = new Color(1f,1f,1f, 1f - updateTicks/maxTime);
            }

            if (updateTicks > maxTime) {
                Destroy(this);
            }
        }
    }
}
