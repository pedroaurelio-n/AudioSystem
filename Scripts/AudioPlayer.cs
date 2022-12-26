using System;
using System.Collections;
using UnityEngine;
 
namespace PedroAurelio.AudioSystem
{
    public class AudioPlayer : MonoBehaviour
    {
        public int ID { get => _id; set => _id = value; }

        private AudioSource _audioSource;
        private AudioClipSO _clipSO;
        private AudioClip _clip;

        private int _id;
        private bool _isPlaying;

        private void Awake() => _audioSource = GetComponent<AudioSource>();

        public void DisableAudioPlayer()
        {
            _audioSource.playOnAwake = false;
            _audioSource.enabled = false;
        }

        public void PlayAudio(AudioClipSO clipSO, float fadeInDuration, Vector3 position, Action releaseAction)
        {
            _clipSO = clipSO;
            _clip = _clipSO.GetClip();

            if (_clip == null)
                return;

            transform.position = position;

            _audioSource.enabled = true;
            _audioSource.clip = _clip;
            _audioSource.loop = _clipSO.Loop;
            _audioSource.spatialBlend = _clipSO.SpatialBlend;
            _audioSource.pitch = clipSO.Pitch;
            _audioSource.outputAudioMixerGroup = _clipSO.MixerGroup;

            if (fadeInDuration == 0f)
                _audioSource.volume = clipSO.Volume;
            else
            {
                _audioSource.volume = 0f;
                StartCoroutine(FadeVolumeCoroutine(clipSO.Volume, fadeInDuration));
            }

            _audioSource.Play();

            _isPlaying = true;
            
            if (!_audioSource.loop)
                StartCoroutine(StopAfterClipCoroutine(releaseAction));
        }

        private IEnumerator StopAfterClipCoroutine(Action releaseAction)
        {
            yield return new WaitForSeconds(_clip.length);
            
            DisableAudio(releaseAction);
        }

        public void StopAudio(float fadeDuration, Action releaseAction)
        {
            if (!_isPlaying)
                return;
            
            if (fadeDuration <= 0f)
                DisableAudio(releaseAction);
            else
                StartCoroutine(FadeVolumeCoroutine(0f, fadeDuration, releaseAction));
        }

        private IEnumerator FadeVolumeCoroutine(float targetVolume, float fadeDuration, Action releaseAction = null)
        {
            var timeElapsed = 0f;
            var startVolume = _audioSource.volume;

            while (timeElapsed < fadeDuration)
            {
                _audioSource.volume = Mathf.Lerp(startVolume, targetVolume, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            _audioSource.volume = targetVolume;

            if (targetVolume == 0f)
                DisableAudio(releaseAction);
        }

        private void DisableAudio(Action releaseAction)
        {
            _clipSO.RemoveInstance();
            _clipSO = null;
            _isPlaying = false;

            releaseAction.Invoke();
        }

        private void OnDisable()
        {
            if (_clipSO != null)
                _clipSO.RemoveInstance();
        }
    }
}