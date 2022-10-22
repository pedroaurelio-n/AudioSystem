using UnityEngine;
 
namespace PedroAurelio.AudioSystem
{
    public class PlayAudioEvent : MonoBehaviour
    {
        [SerializeField] private AudioEventChannelSO audioChannel;
        [SerializeField] private AudioClipSO clipSO;
        [SerializeField] private bool playOnStart;
        [SerializeField] private float delay;

        private void Start()
        {
            if (playOnStart)
                audioChannel.RaiseEvent(clipSO, transform.position, delay);
        }

        public void PlayAudio() => audioChannel.RaiseEvent(clipSO, transform.position, delay);
    }
}