using BlackGardenStudios.HitboxStudioPro;
using UnityEngine;

namespace TeamRitual.Core {
public class SoundHandler {
    public AudioSource[] audioSources;
    public SoundHandler(AudioSource[] audioSources) {
        this.audioSources = audioSources;
    }

    public void PlaySound(AudioClip clip, bool stopOthers) {
        if (clip == null) {
            return;
        }

        if (stopOthers) {
            foreach (AudioSource audioSource in audioSources) {
                audioSource.Stop();
            }
            audioSources[0].clip = clip;
            audioSources[0].Play();
        } else {
            foreach (AudioSource audioSource in audioSources) {
                if (!audioSource.isPlaying) {
                    audioSource.PlayOneShot(clip);
                    return;
                }
            }
            audioSources[audioSources.Length-1].PlayOneShot(clip);
        }
    }
}
}