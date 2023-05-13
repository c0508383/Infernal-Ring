using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamRitual.UI {
public class MainMenuSun : MonoBehaviour
{
    public List<Sprite> Sprites = new List<Sprite>();
	public float AnimSpeed = 0.5f;
	public int FrameRate = 60;
	private Image Image;
	private float mTimePerFrame;
	private float mElapsedTime;
	private int mCurrentFrame;

	private void Start()
	{
		this.Image = this.GetComponent<Image>();
		this.LoadSpriteSheet();
	}

	public void LoadSpriteSheet()
	{
		this.Sprites.Clear();
        for (int i = 0; i < 100; i++) {
		    this.Sprites.Add(UnityEngine.Resources.Load<Sprite>("Sprites/MainMenu/Sun/"+i));
        }
        for (int i = 87; i >= 85; i--) {
		    this.Sprites.Add(UnityEngine.Resources.Load<Sprite>("Sprites/MainMenu/Sun/"+i));
        }

		this.mTimePerFrame = 1f / (float) this.FrameRate;
	}

	private void Update()
	{
		this.mElapsedTime += Time.deltaTime * this.AnimSpeed;
		if ((double) this.mElapsedTime < (double) this.mTimePerFrame * (double) this.mCurrentFrame)
			return;
		this.NextSprite();
		++this.mCurrentFrame;
	}

	private void NextSprite()
	{
		if (this.mCurrentFrame >= this.Sprites.Count)
			Reset();
		if (this.mCurrentFrame < 0)
			return;
		this.Image.sprite = this.Sprites[this.mCurrentFrame];
	}

    void Reset() {
	    this.mElapsedTime = 0;
	    this.mCurrentFrame = 0;
    }
}
}