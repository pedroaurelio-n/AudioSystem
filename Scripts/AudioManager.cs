using UnityEngine;
using UnityEngine.Pool;
 
namespace PedroAurelio.AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioEventChannelSO sfxChannel;
        [SerializeField] private AudioEventChannelSO musicChannel;
        [SerializeField] private AudioEventChannelSO uiChannel;
        [SerializeField] private int initialPoolCount;

        private ObjectPool<AudioPlayer> _audioPlayerPool;

        private void Awake()
        {
            _audioPlayerPool = new ObjectPool<AudioPlayer>(OnCreateAudioPlayer, OnGetAudioPlayer, OnReleaseAudioPlayer);

            InitializePool(initialPoolCount);
        }

        #region Pool Methods
        private AudioPlayer OnCreateAudioPlayer()
        {
            var newPlayer = new GameObject("AudioPlayer");
            newPlayer.transform.SetParent(transform);

            newPlayer.AddComponent<AudioSource>();
            var audioPlayer = newPlayer.AddComponent<AudioPlayer>();

            return audioPlayer;
        }

        private void OnGetAudioPlayer(AudioPlayer audioPlayer) => audioPlayer.gameObject.SetActive(true);

        private void OnReleaseAudioPlayer(AudioPlayer audioPlayer)
        {
            audioPlayer.DisableAudioPlayer();
            audioPlayer.gameObject.SetActive(false);
        }

        private void InitializePool(int count)
        {
            var audioPlayers = new AudioPlayer[count];

            for (int i = 0; i < count; i++)
            {
                var temp = _audioPlayerPool.Get();
                audioPlayers[i] = temp;
            }

            for (int i = audioPlayers.Length - 1; i >= 0; i--)
                _audioPlayerPool.Release(audioPlayers[i]);
        }
        #endregion

        private void PlayAudio(AudioClipSO clipSO, Vector3 position, float delay)
        {
            if (!clipSO.CanActivateNewInstance())
                return;
            
            clipSO.AddInstance();
            var audioPlayer = _audioPlayerPool.Get();
            audioPlayer.PlayAudio(clipSO, position, delay, () => _audioPlayerPool.Release(audioPlayer));
        }

        private void OnEnable()
        {
            sfxChannel.onRaiseAudio += PlayAudio;
            musicChannel.onRaiseAudio += PlayAudio;
            uiChannel.onRaiseAudio += PlayAudio;
        }

        private void OnDisable()
        {
            sfxChannel.onRaiseAudio -= PlayAudio;
            musicChannel.onRaiseAudio -= PlayAudio;
            uiChannel.onRaiseAudio -= PlayAudio;
        }
    }
}