using UnityEngine;
using UnityEngine.Audio;
 
namespace PedroAurelio.AudioSystem
{
    [CreateAssetMenu(fileName = "New Audio Clip", menuName = "Audio/Audio Clip")]
    public class AudioClipSO : ScriptableObject
    {
        [Header("Audio Dependencies")]
        public AudioClip Clip;
        public AudioMixerGroup MixerGroup;

        [Header("Clip Settings")]
        public int MaxInstances;

        [Header("Audio Source Settings")]
        public bool Loop;
        [Range(0f, 1f)] public float SpatialBlend;

        private int _currentInstances;

        public bool CanActivateNewInstance()
        {
            if (MaxInstances <= 0)
                MaxInstances = int.MaxValue;
            
            return _currentInstances < MaxInstances;
        }

        public void AddInstance() => _currentInstances++;
        public void RemoveInstance() => _currentInstances--;
    }
}