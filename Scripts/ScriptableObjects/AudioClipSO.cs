using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
 
namespace PedroAurelio.AudioSystem
{
    [CreateAssetMenu(fileName = "New Audio Clip", menuName = "Audio/Audio Clip")]
    public class AudioClipSO : ScriptableObject
    {
        [Header("Audio Dependencies")]
        public List<AudioClip> Clips;
        public AudioMixerGroup MixerGroup;

        [Header("Clip Settings")]
        public int MaxInstances = 3;

        [Header("Source Settings")]
        public bool Loop = false;
        [Range(0f, 1f)] public float SpatialBlend = 0f;
        [SerializeField] private Vector2 volumeRange = Vector2.one;
        [SerializeField] private Vector2 pitchRange = Vector2.one;
        public float Volume => Random.Range(volumeRange.x, volumeRange.y);
        public float Pitch => Random.Range(pitchRange.x, pitchRange.y);

        private int _currentInstances;

        private void OnValidate()
        {
            ClampVolume();
            ClampPitch();
        }

        private void ClampVolume()
        {
            volumeRange.x = Mathf.Clamp(volumeRange.x, 0f, volumeRange.y);
            volumeRange.y = Mathf.Clamp(volumeRange.y, volumeRange.x, 1f);
        }

        private void ClampPitch()
        {
            pitchRange.x = Mathf.Clamp(pitchRange.x, -3f, pitchRange.y);
            pitchRange.y = Mathf.Clamp(pitchRange.y, pitchRange.x, 3f);
        }

        public AudioClip GetClip()
        {
            if (Clips.Count == 0)
            {
                Debug.LogError($"At least 1 AudioClip is needed.");
                return null;
            }

            if (Clips.Count == 1)
                return Clips[0];

            else
            {
                var randomIndex = Random.Range(0, Clips.Count);
                return Clips[randomIndex];
            }
        }

        public bool CanActivateNewInstance()
        {
            if (MaxInstances <= 0)
                MaxInstances = int.MaxValue;
            
            return _currentInstances < MaxInstances;
        }

        public void AddInstance() => _currentInstances++;
        public void RemoveInstance() => _currentInstances--;
        private void ResetInstances() => _currentInstances = 0;

        [UnityEditor.CustomEditor(typeof(AudioClipSO))]
        public class AudioClipSOEditor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                var audioClipSO = (AudioClipSO)target;

                if (GUILayout.Button("Reset Instances"))
                    audioClipSO.ResetInstances();
            }
        }
    }
}