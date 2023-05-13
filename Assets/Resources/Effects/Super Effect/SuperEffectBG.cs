using BlackGardenStudios.HitboxStudioPro;
using System.Collections;
using System.Collections.Generic;
using TeamRitual.Core;
using UnityEngine;

namespace TeamRitual {
public class SuperEffectBG : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    float scale = 100f;
    float updateTicks;
    float maxTime;
    
    // Start is called before the first frame update
    void Start()
    {
        updateTicks = 0;
        maxTime = 60*10;
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector2(scale,scale);
        spriteRenderer.color = new Color(1f,1f,1f,1f);
    }

    void FixedUpdate()
    {
        updateTicks++;
        if (updateTicks > 20) {
            spriteRenderer.color = new Color(1f,1f,1f,1f - (updateTicks-20)/(maxTime-20));
        }
        if (updateTicks > maxTime) {
            Destroy(this);
        }
    }
}
}
