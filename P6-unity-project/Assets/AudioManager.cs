    using UnityEngine;
    using UnityEngine.Audio;
    using System.Collections;

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public AudioMixer mixer;
        public AudioSource musicSource;
        public string musicVolumeParam = "MusicVolume";
        public float fadeDuration = 1.0f;

        [Range(-80f, 0f)] public float targetVolume = 0f; // 0 dB = full volume
        [Range(-80f, 0f)] public float muteVolume = -80f;

        private Coroutine currentFadeCoroutine;

        public string radioEQParam = "RadioEQ";
        public float radioEQOnGain = 0f;      // 0 dB
        public float radioEQOffGain = -80f;   // Essentially muted

    private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }

            mixer.SetFloat(musicVolumeParam, muteVolume);
        }

        public void FadeGameTheme(bool fadeIn, bool instant = false)
        {
            if (currentFadeCoroutine != null)
                StopCoroutine(currentFadeCoroutine);

            if (instant)
            {
                mixer.SetFloat(musicVolumeParam, fadeIn ? targetVolume : muteVolume);
            }
            else
            {
                currentFadeCoroutine = StartCoroutine(FadeMixerGroupVolume(fadeIn));
            }
        }

        public void ToggleRadioEQ(bool on)
        {
        float gain = on ? radioEQOnGain : radioEQOffGain;
        mixer.SetFloat(radioEQParam, gain);
        }

    private IEnumerator FadeMixerGroupVolume(bool fadeIn)
        {
            mixer.GetFloat(musicVolumeParam, out float startVolume);
            float endVolume = fadeIn ? targetVolume : muteVolume;

            float time = 0f;
            while (time < fadeDuration)
            {
                float newVolume = Mathf.Lerp(startVolume, endVolume, time / fadeDuration);
                mixer.SetFloat(musicVolumeParam, newVolume);
                time += Time.deltaTime;
                yield return null;
            }

            mixer.SetFloat(musicVolumeParam, endVolume);
        }
    }
