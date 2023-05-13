using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamRitual
{
    public class Airdash_00 : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        float scale = 4.0f;
        float updateTicks;
        float maxTime;
        // Start is called before the first frame update
        void Start()
        {
            updateTicks = 0;
            maxTime = 16*10;
            spriteRenderer = GetComponent<SpriteRenderer>();
            transform.localScale = new Vector2(scale,scale);
            GetComponent<Animator>().Play("airdash");
        }

        void FixedUpdate()
        {
            updateTicks++;
            
            spriteRenderer.color = new Color(1f,1f,1f, 1f - updateTicks/maxTime);

            if (updateTicks > maxTime) {
                Destroy(this);
            }
        }
    }
}
