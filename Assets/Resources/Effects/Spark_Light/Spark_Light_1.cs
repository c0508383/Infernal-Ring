using UnityEngine;

namespace TeamRitual
{
    public class Spark_Light_1 : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        float scale = 0.2f;
        float updateTicks;
        float maxTime;
        // Start is called before the first frame update
        void Start()
        {
            updateTicks = 0;
            maxTime = 12*10;
            spriteRenderer = GetComponent<SpriteRenderer>();
            transform.localScale = new Vector2(scale,scale);
        }

        void FixedUpdate()
        {
            updateTicks++;
            transform.localScale = new Vector2(scale*(1 + 2f*(updateTicks/maxTime)), scale*(1 + 2f*(updateTicks/maxTime)));
            spriteRenderer.color = new Color(1f,1f,1f, 1f - updateTicks/maxTime);

            if (updateTicks > maxTime) {
                Destroy(this);
            }
        }
    }
}
