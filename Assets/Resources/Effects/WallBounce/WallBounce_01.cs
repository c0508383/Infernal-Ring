using UnityEngine;

namespace TeamRitual
{
    public class WallBounce_01 : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        float scale = 1.5f;
        float updateTicks;
        float maxTime;
        // Start is called before the first frame update
        void Start()
        {
            updateTicks = 0;
            maxTime = 15*10;
            spriteRenderer = GetComponent<SpriteRenderer>();
            transform.localScale = new Vector2(scale,scale);
            this.GetComponent<Animator>().Play("wallbounce");
        }

        void FixedUpdate()
        {
            updateTicks++;

            transform.localScale = new Vector2(scale*(1 + 1f*(updateTicks/maxTime)), scale*(1 + 1f*(updateTicks/maxTime)));

            spriteRenderer.color = new Color(1f,1f,1f, 1f - updateTicks/maxTime);

            if (updateTicks > maxTime) {
                Destroy(this);
            }
        }
    }
}
