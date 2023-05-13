using System.Collections.Generic;
using TeamRitual.Core;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Round : MonoBehaviour
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
		foreach (Sprite sprite in UnityEngine.Resources.LoadAll<Sprite>("Sprites/Versus/HUD/Round/roundstart"))
			this.Sprites.Add(sprite);
		Sprite[] spriteArray = UnityEngine.Resources.LoadAll<Sprite>("Sprites/Versus/HUD/Round/round" + GameController.Instance.currentRound.ToString());
		foreach (Sprite sprite in spriteArray)
			this.Sprites.Add(sprite);
		for (int index = 0; index < 5; ++index)
			this.Sprites.Add(spriteArray[spriteArray.Length - 1]);
		this.mTimePerFrame = 1f / (float) this.FrameRate;
		++GameController.Instance.currentRound;
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
