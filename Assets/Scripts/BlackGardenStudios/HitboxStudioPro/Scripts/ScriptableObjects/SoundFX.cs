using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackGardenStudios.HitboxStudioPro
{
    [CreateAssetMenu()]
    public class SoundFX : ScriptableObject
    {
        public AudioClip[] Effects;
        public int uniqueID;
    }
}