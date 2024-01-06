using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RPG.Audio
{
	public class AudioController : Singleton<AudioController>
    {
        /************************************************************************************************************************/
        #region Declaration
        [SerializeField] private AudioSource defaultAudioSource;
        
        private HashSet<AudioClip> _recentClips = new HashSet<AudioClip>();
        private HashSet<(AudioList, AudioGroup)> _recentGroups = new HashSet<(AudioList, AudioGroup)>();
        
        private const float RepeatTolerance = 0.1f;
        #endregion
        /************************************************************************************************************************/
        #region Commands
        public void PlayAudioFade(AudioList audioList, AudioGroup group, float fadeDuration = 1f, AudioSource source = null)
        {
            StartCoroutine(AudioFadeCoroutine(audioList, group, fadeDuration, source));
        }
        public void PlayAudioFade(AudioClip clip, float fadeDuration = 1f, AudioSource source = null)
        {
            StartCoroutine(AudioFadeCoroutine(clip, fadeDuration, source));
        }
        private IEnumerator AudioFadeCoroutine(AudioClip clip, float fadeDuration = 1f, AudioSource source = null)
        {
            if (source == null)
            {
                source = defaultAudioSource;
                if (defaultAudioSource == null) { yield break; }
            }
            source.Play();

            var currentTime = 0f;
            
            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                source.volume = Mathf.Lerp(0, source.volume, currentTime / fadeDuration);
                yield return null;
            }
        }
        private IEnumerator AudioFadeCoroutine(AudioList audioList, AudioGroup group, float fadeDuration, AudioSource source = null)
	    {
		    if(audioList == null) { yield break; }
		    
            if (source == null)
            {
                source = defaultAudioSource;
                if (defaultAudioSource == null) { yield break; }
            }

            source.clip = audioList.GetRandomClip(group);
            source.Play();

            var currentTime = 0f;
            
            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                source.volume = Mathf.Lerp(0, source.volume, currentTime / fadeDuration);
                yield return null;
            }
        }
        
        public IEnumerator FadeSource(float duration, AudioSource source = null)
        {
            if (source == null)  { yield break; } 

            source.Play();
            var startVolume = 0f;
            var currentVolume = startVolume;
            var targetVolume = source.volume;
            var currentTime = 0f;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
                yield return null;
            }
        }
        public IEnumerator StopAudioFade(float duration, AudioSource source = null)
        {
            if (source == null)  { yield break; } 

            var startVolume = source.volume;
            var currentVolume = startVolume;
            var targetVolume = 0f;
            var currentTime = 0f;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
                yield return null;
            }
            source.Stop();
            source.volume = currentVolume;
        }

        public void PlaySound(AudioClip clip, float volume = 1f, AudioSource source = null, float repeatTolerance = RepeatTolerance)
        {
            if (clip == null || _recentClips.Contains(clip)) { return; }
            
            if (source == null)
            {
                source = defaultAudioSource;
                if (defaultAudioSource == null) { return; }
            }

            _recentClips.Add(clip);
            Invoker.Invoke(this, () => _recentClips.Remove(clip), repeatTolerance, true);
            source.PlayOneShot(clip, volume);
        }
	    public AudioClip PlayAudioListSound(AudioList audioList, AudioGroup group, float volume = 1f, 
            AudioSource source = null, float repeatTolerance = RepeatTolerance)
	    {
            if(audioList == null || _recentGroups.Contains((audioList, group))) { return null; }
		    
            if (source == null)
            {
                source = defaultAudioSource;
                if (defaultAudioSource == null) { return null; }
            }
            
            _recentGroups.Add((audioList, group));
            Invoker.Invoke(this, () => _recentGroups.Remove((audioList, group)), repeatTolerance, true);

		    var audioClip = audioList.GetRandomClip(group);
            PlaySound(audioClip, volume, source, repeatTolerance);
            
            return audioClip;
        }
        #endregion
        /************************************************************************************************************************/
    }
}