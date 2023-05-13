using BlackGardenStudios.HitboxStudioPro;
using System.Collections;
using System.Collections.Generic;
using TeamRitual.Core;
using UnityEngine;

namespace TeamRitual {
public class SuperEffect : MonoBehaviour
{
    Animator anim;
    SpriteRenderer spriteRenderer;
    float scale = 5f;
    float updateTicks;
    float maxTime;
    
    // Start is called before the first frame update
    void Start()
    {
        updateTicks = 0;
        maxTime = 10*10;
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector2(scale,scale);
        spriteRenderer.color = new Color(1f,1f,1f,1f);
        anim = GetComponent<Animator>();
        anim.Play("superflash-0");
        GameController.Instance.soundHandler.PlaySound(EffectSpawner.GetSoundEffect(110), true);
        EffectSpawner.PlayHitEffect(1201, transform.position, this.spriteRenderer.sortingOrder - 2, true);
    }

    void FixedUpdate()
    {
        updateTicks++;
        if (updateTicks > maxTime || anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) {
            spriteRenderer.color = new Color(1f,1f,1f,0f);
            Destroy(this);
        }
    }
}
}
