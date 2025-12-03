using UnityEngine;

namespace _Project.Systems.Audio
{
    public class AudioSystem
    {
        private readonly AudioSource _musicSource;
        private readonly AudioSource _sfxSource;

        public AudioSystem()
        {
            var go = new GameObject("AudioSystem");
            Object.DontDestroyOnLoad(go);

            _musicSource = go.AddComponent<AudioSource>();
            _sfxSource = go.AddComponent<AudioSource>();

            _musicSource.loop = true;
        }

        public void PlayMusic(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            _musicSource.clip = clip;
            _musicSource.volume = volume;
            _musicSource.Play();
        }

        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            _sfxSource.PlayOneShot(clip, volume);
        }

        public void StopMusic() => _musicSource.Stop();
    }
}
