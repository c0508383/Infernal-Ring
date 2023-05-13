using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_KO : MonoBehaviour
{
	public static KOType KOType;
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
		this.enabled = false;
		this.Play();
	}

	public void Play()
	{
		this.LoadSpriteSheet();
		this.enabled = true;
	}

	public void LoadSpriteSheet()
	{
		string str = "ko";
		switch (HUD_KO.KOType)
		{
			case KOType.DOUBLE:
				str = "dko";
				break;
			case KOType.TIME:
				str = "time";
				break;
		}
		this.Sprites.Clear();
		for (int index = 0; index <= 1; ++index)
			this.Sprites.Add(UnityEngine.Resources.Load<Sprite>("Sprites/Versus/HUD/KO/" + str + "-0"));
		for (int index = 0; index <= 10; ++index)
			this.Sprites.Add(UnityEngine.Resources.Load<Sprite>("Sprites/Versus/HUD/KO/" + str + "-1"));
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
