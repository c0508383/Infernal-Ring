// Decompiled with JetBrains decompiler
// Type: BlackGardenStudios.HitboxStudioPro.EffectSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EFEF0DDB-8E16-4DAE-8226-188826837CED
// Assembly location: C:\Users\luisc\OneDrive\Desktop\Infernal Ring Build\Infernal Ring_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlackGardenStudios.HitboxStudioPro
{
	public class EffectSpawner : MonoBehaviour
	{
		public static AttackFX[] m_Effects;
		public static SoundFX[] m_SoundEffects;
		public static Dictionary<int, AttackFX> m_EffectDict = new Dictionary<int, AttackFX>();
		public static Dictionary<int, SoundFX> m_SoundEffectDict = new Dictionary<int, SoundFX>();
		public static EffectSpawner m_Instance;
		private WaitForSeconds m_Wait = new WaitForSeconds(1f);

		/*private static EffectSpawner instance
		{
			get
			{
				if ((UnityEngine.Object) EffectSpawner.m_Instance == (UnityEngine.Object) null)
				{
					GameObject gameObject = new GameObject("Effect Spawner", new Type[1]
					{
						typeof (EffectSpawner)
					});
				}
				return EffectSpawner.m_Instance;
			}
			set => EffectSpawner.m_Instance = value;
		}*/

		private static EffectSpawner instance
		{
			get
			{
				if (m_Instance == null)
				{
					m_Instance = FindObjectOfType<EffectSpawner>();
					if (m_Instance == null)
					{
						GameObject singletonObject = new GameObject();
						m_Instance = singletonObject.AddComponent<EffectSpawner>();
						singletonObject.name = typeof(EffectSpawner).ToString() + " (Singleton)";
						DontDestroyOnLoad(singletonObject);
					}
				}
				return m_Instance;
			}
		}

		public static GameObject PlayHitEffect(int uid, Vector3 point, int order, bool flipx) => EffectSpawner.instance._PlayHitEffect(uid, point, order, flipx, new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte) 1));

		public static GameObject PlayHitEffect(
			int uid,
			Vector3 point,
			int order,
			bool flipx,
			Color32 color)
		{
			return EffectSpawner.instance._PlayHitEffect(uid, point, order, flipx, color);
		}

		public GameObject _PlayHitEffect(int uid, Vector3 point, int order, bool flipx, Color32 color)
		{
			AttackFX attackFx;
			if (!EffectSpawner.m_EffectDict.TryGetValue(uid, out attackFx) || attackFx.Effects == null || attackFx.Effects.Length == 0)
				return (GameObject) null;
			GameObject go = UnityEngine.Object.Instantiate<GameObject>(attackFx.Effects[UnityEngine.Random.Range(0, attackFx.Effects.Length)], point, Quaternion.identity);
			SpriteRenderer component = go.GetComponent<SpriteRenderer>();
			if ((UnityEngine.Object) component != (UnityEngine.Object) null)
			{
				component.flipX = flipx;
				component.sortingOrder = order;
				component.color = (Color) color;
			}
			this.StartCoroutine(this.DestroyEffect(go));
			return go;
		}

		public static AudioClip GetSoundEffect(int uid) => EffectSpawner.instance._GetSoundEffect(uid);

		public AudioClip _GetSoundEffect(int uid)
		{
			SoundFX soundFx;
			return EffectSpawner.m_SoundEffectDict.TryGetValue(uid, out soundFx) && soundFx.Effects != null && soundFx.Effects.Length != 0 ? soundFx.Effects[UnityEngine.Random.Range(0, soundFx.Effects.Length)] : (AudioClip) null;
		}

		private IEnumerator DestroyEffect(GameObject go)
		{
			yield return (object) this.m_Wait;
			UnityEngine.Object.Destroy((UnityEngine.Object) go);
		}

		private void Awake()
		{
			if ((UnityEngine.Object) EffectSpawner.m_Instance == (UnityEngine.Object) null)
			{
				EffectSpawner.m_EffectDict.Clear();
				EffectSpawner.m_SoundEffectDict.Clear();
				EffectSpawner.m_Instance = this;
				EffectSpawner.m_Effects = Resources.LoadAll<AttackFX>("Effects/");
				EffectSpawner.m_SoundEffects = Resources.LoadAll<SoundFX>("Sounds/");
				for (int index = 0; index < EffectSpawner.m_Effects.Length; ++index)
					EffectSpawner.m_EffectDict.Add(EffectSpawner.m_Effects[index].uniqueID, EffectSpawner.m_Effects[index]);
				for (int index = 0; index < EffectSpawner.m_SoundEffects.Length; ++index)
					EffectSpawner.m_SoundEffectDict.Add(EffectSpawner.m_SoundEffects[index].uniqueID, EffectSpawner.m_SoundEffects[index]);
			}
			else
				UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
		}

		public static void RefreshData()
		{
			EffectSpawner.m_EffectDict.Clear();
			EffectSpawner.m_SoundEffectDict.Clear();
			EffectSpawner.m_Effects = Resources.LoadAll<AttackFX>("Effects/");
			EffectSpawner.m_SoundEffects = Resources.LoadAll<SoundFX>("Sounds/");
			for (int index = 0; index < EffectSpawner.m_Effects.Length; ++index)
				EffectSpawner.m_EffectDict.Add(EffectSpawner.m_Effects[index].uniqueID, EffectSpawner.m_Effects[index]);
			for (int index = 0; index < EffectSpawner.m_SoundEffects.Length; ++index)
				EffectSpawner.m_SoundEffectDict.Add(EffectSpawner.m_SoundEffects[index].uniqueID, EffectSpawner.m_SoundEffects[index]);
		}

		public static AttackFX[] GetPools()
		{
			List<AttackFX> list = ((IEnumerable<AttackFX>) Resources.LoadAll<AttackFX>("Effects/")).ToList<AttackFX>();
			list.Sort((Comparison<AttackFX>) ((a, b) => b.uniqueID - a.uniqueID));
			return list.ToArray();
		}

		public static SoundFX[] GetSoundPools()
		{
			List<SoundFX> list = ((IEnumerable<SoundFX>) Resources.LoadAll<SoundFX>("Sounds/")).ToList<SoundFX>();
			list.Sort((Comparison<SoundFX>) ((a, b) => b.uniqueID - a.uniqueID));
			return list.ToArray();
		}

		private void OnDestroy()
		{
			if (!((UnityEngine.Object) EffectSpawner.m_Instance == (UnityEngine.Object) this))
				return;
			EffectSpawner.m_Instance = (EffectSpawner) null;
		}
	}
}
