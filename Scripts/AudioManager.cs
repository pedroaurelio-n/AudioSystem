using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
namespace PedroAurelio.AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Settings")]
        [SerializeField] private int initialPoolCount;

        private List<AudioPlayer> _audioPlayerPool;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);

            _audioPlayerPool = new List<AudioPlayer>();
            InitializePool(initialPoolCount);
        }

        #region Pool Methods
        private AudioPlayer CreateAudioPlayer()
        {
            var newPlayer = new GameObject("AudioPlayer");
            newPlayer.transform.SetParent(transform);

            newPlayer.AddComponent<AudioSource>();
            var audioPlayer = newPlayer.AddComponent<AudioPlayer>();

            _audioPlayerPool.Add(audioPlayer);
            return audioPlayer;
        }

        private AudioPlayer GetAudioPlayer()
        {
            foreach (AudioPlayer audioPlayer in _audioPlayerPool)
            {
                if (!audioPlayer.gameObject.activeInHierarchy)
                {
                    audioPlayer.gameObject.SetActive(true);
                    return audioPlayer;
                }
            }

            return CreateAudioPlayer();
        }

        private void ReleaseAudioPlayer(AudioPlayer audioPlayer)
        {
            audioPlayer.DisableAudioPlayer();
            audioPlayer.gameObject.SetActive(false);
        }

        private void InitializePool(int count)
        {
            for (int i = 0; i < count; i++)
                CreateAudioPlayer();

            for (int i = _audioPlayerPool.Count - 1; i >= 0; i--)
                ReleaseAudioPlayer(_audioPlayerPool[i]);
        }
        #endregion

        private int GenerateID()
        {
            int randomID;
            var isIDRepeated = false;

            do
            {
                randomID = Random.Range(0, int.MaxValue);
                var player = IsAudioPlayerUsingID(randomID);
                isIDRepeated = player != null;
            } while (isIDRepeated);

            return randomID;
        }

        private AudioPlayer IsAudioPlayerUsingID(int id)
        {
            foreach (AudioPlayer audioPlayer in _audioPlayerPool)
            {
                if (audioPlayer.ID != id)
                    continue;
                
                else
                    return audioPlayer;
            }

            return null;
        }

        public void RequestAudioPlayer(out int id, AudioClipSO clipSO, float fadeInDuration, Vector3 position, float delay)
        {
            id = -1;

            if (!clipSO.CanActivateNewInstance())
                return;
            
            id = GenerateID();
            clipSO.AddInstance();
            var audioPlayer = GetAudioPlayer();
            audioPlayer.ID = id;

            StartCoroutine(CheckAudioDelay(audioPlayer, clipSO, fadeInDuration, position, delay));
        }

        public void StopAudioPlayer(int id, float fadeOutDuration)
        {
            var audioPlayer = IsAudioPlayerUsingID(id);

            if (audioPlayer != null)
                audioPlayer.StopAudio(fadeOutDuration, () => ReleaseAudioPlayer(audioPlayer));
        }

        private IEnumerator CheckAudioDelay(AudioPlayer audioPlayer, AudioClipSO clipSO, float fadeInDuration, Vector3 position, float delay)
        {
            if (delay == 0f)
                audioPlayer.PlayAudio(clipSO, fadeInDuration, position, () => ReleaseAudioPlayer(audioPlayer));
            else
            {
                yield return new WaitForSeconds(delay);
                audioPlayer.PlayAudio(clipSO, fadeInDuration, position, () => ReleaseAudioPlayer(audioPlayer));
            }
        }
    }
}