using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Fight : MonoBehaviour
{
	public List<Sprite> Sprites = new List<Sprite>();
	public float AnimSpeed = 1f;
	public int FrameRate = 30;
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
		Sprite[] spriteArray = UnityEngine.Resources.LoadAll<Sprite>("Sprites/Versus/HUD/fight");
		for (int index = 0; index < 3; ++index)
		{
			this.Sprites.Add(spriteArray[0]);
			this.Sprites.Add(spriteArray[1]);
		}
		for (int index = 0; index < 6; ++index)
			this.Sprites.Add(spriteArray[2]);
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
		this.Image.color = (Color) new Color32((byte) 1, (byte) 1, (byte) 1, (byte) 0);
		this.enabled = false;
	}

	private void NextSprite()
	{
		if (this.mCurrentFrame < 0 || this.mCurrentFrame >= this.Sprites.Count)
			return;
		this.Image.sprite = this.Sprites[this.mCurrentFrame];
	}
}
