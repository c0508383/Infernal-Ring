using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamRitual.UI {
public class CageAnimation : MonoBehaviour
{
	public List<Sprite> Sprites = new List<Sprite>();
	public float AnimSpeed = 1f;
	public int FrameRate = 30;
	private Image Image;
	private float mTimePerFrame;
	private float mElapsedTime;
	private int mCurrentFrame;

    void Start()
    {
		this.Image = this.GetComponent<Image>();
		this.enabled = false;
    }

	public void Play()
	{
		this.LoadSpriteSheet();
		this.enabled = true;
	}

	public void LoadSpriteSheet()
	{
		this.Sprites.Clear();
		for (int index = 1; index <= 11; ++index)
			this.Sprites.Add(UnityEngine.Resources.Load<Sprite>("Sprites/CharacterSelect/Bars/Bars" + index));
		this.mTimePerFrame = 1f / (float) this.FrameRate;
	}

	private void Update()
	{
		this.mElapsedTime += Time.deltaTime * this.AnimSpeed;
		if ((double) this.mElapsedTime < (double) this.mTimePerFrame * (double) this.mCurrentFrame)
			return;
		this.NextSprite();
		++this.mCurrentFrame;
		if (this.mCurrentFrame < this.Sprites.Count)
			return;
		this.enabled = false;
	}

	private void NextSprite()
	{
		if (this.mCurrentFrame < 0 || this.mCurrentFrame >= this.Sprites.Count)
			return;
		this.Image.sprite = this.Sprites[this.mCurrentFrame];
	}
}
}