using UnityEngine;
using DG.Tweening;

namespace Aurinaxtailer
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private float timeFadeChangeMusic;
        private AudioSource audioSource;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound(AudioClip audioClip)
        {
            if (audioClip == null)
                return;
            audioSource.PlayOneShot(audioClip);
        }

        public void PlayMusic(AudioClip audioClip)
        {
            if (audioClip == null)
                return;
            if (audioSource.isPlaying && audioSource.clip == audioClip)
                return;
            audioSource
                .DOFade(0, timeFadeChangeMusic)
                .OnComplete(() =>
                {
                    audioSource.clip = audioClip;
                    audioSource.Play();
                    audioSource.DOFade(0.5f, timeFadeChangeMusic);
                });
        }
    }
}
