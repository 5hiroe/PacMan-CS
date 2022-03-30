using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class AnimatedSprite : MonoBehaviour
{
    //private setter because we may call an AnimatedSprite in another script, so it's better to make this setter private.
    public SpriteRenderer spriteRenderer { get; private set; } 
    public Sprite[] sprites;
    public float animationTime = 0.125f; //Next sprite every 0.125 seconds
    //People have to be able to read (get) the animation Frame, but not set it
    public int animationFrame { get; private set; }
    public bool loop = true; //To be able to turn the loop on and off depending of the animation

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(Advance), this.animationTime, this.animationTime);
    }

    private void Advance()
    {
        //If animation is disabled
        if (!this.spriteRenderer.enabled){
            return;
        }

        this.animationFrame++;

        //Loop function
        if (this.animationFrame >= this.sprites.Length && this.loop) {
            this.animationFrame = 0;
        }

        if (this.animationFrame >= 0 && this.animationFrame < this.sprites.Length)  {
            this.spriteRenderer.sprite = this.sprites[this.animationFrame];
        }
    }

    public void Restart()
    {
        this.animationFrame = -1;

        Advance();
    }
}
