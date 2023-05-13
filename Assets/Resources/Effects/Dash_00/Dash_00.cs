using TeamRitual.Core;
using UnityEngine;

namespace TeamRitual
{
    public class Dash_00 : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        Animator animator;

        float scale = 1.2f;
        string animationName = "dash_0";

        void Start()
        {
            this.spriteRenderer = GetComponent<SpriteRenderer>();
            this.transform.localScale = new Vector2(this.scale,this.scale);
            this.animator = GetComponent<Animator>();
            this.spriteRenderer.color = new Color(1f,1f,1f,1f);
            this.animator.Play(this.animationName);
        }

        void FixedUpdate()
        {
            if (GameController.Instance.AnimationOver(this.animator,this.animationName)) {
                this.spriteRenderer.color = new Color(1f,1f,1f,0);
                Destroy(this);
            }
        }
    }
}
