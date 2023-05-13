using BlackGardenStudios.HitboxStudioPro;
using UnityEngine;

namespace TeamRitual
{
    public class Spark_Heavy_0 : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        float scale = 0.65f;
        float updateTicks;
        float maxTime;
        // Start is called before the first frame update
        void Start()
        {
            updateTicks = 0;
            maxTime = 20*10;
            spriteRenderer = GetComponent<SpriteRenderer>();
            transform.localScale = new Vector2(scale,scale);
            spriteRenderer.color = new Color(1f,1f,1f,1f);
            EffectSpawner.PlayHitEffect(
                301, transform.position, spriteRenderer.sortingOrder - 1, true
            );
            EffectSpawner.PlayHitEffect(
                52, transform.position, spriteRenderer.sortingOrder - 1, true
            );
        }

        void FixedUpdate()
        {
            updateTicks++;
            transform.localRotation = Quaternion.Euler(0f, 0f, updateTicks*8);
            transform.localScale = new Vector2(scale*(1 - 1f*(updateTicks/maxTime)), scale*(1 - 1f*(updateTicks/maxTime)));
            //spriteRenderer.color = new Color(1f,1f,1f, 1f - updateTicks/maxTime);

            if (updateTicks > maxTime) {
                Destroy(this);
            }
        }
    }
}
