using System;
using System.Collections.Generic;
using UnityEngine;

namespace MC.Core
{
    public partial class AudioManager : MonoBehaviour
    {
        public Action<bool> OnSFXMuteChange;
        public Action<bool> OnMusicMuteChange;
        public Action<float> OnSFXVolumeChange;
        public Action<float> OnMusicVolumeChange;

        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioSource musicSource;

        private bool _isSFXMuted;
        public bool IsSFXMuted
        {
            get
            {
                return _isSFXMuted;
            }
            private set
            {
                _isSFXMuted = value;
                PlayerPrefs.SetInt(GameUtilities.PlayerPrefs.isSFXMuted, _isSFXMuted ? 1 : 0);

                OnSFXMuteChange?.Invoke(value);
            }
        }

        private bool _isMusicMuted;
        public bool IsMusicMuted
        {
            get
            {
                return _isMusicMuted;
            }
            private set
            {
                _isMusicMuted = value;
                PlayerPrefs.SetInt(GameUtilities.PlayerPrefs.isMusicMuted, _isMusicMuted ? 1 : 0);

                OnMusicMuteChange?.Invoke(value);
            }
        }

        private float _sfxVolume;
        public float SFXVolume
        {
            get
            {
                return _sfxVolume;
            }
            set
            {
                _sfxVolume = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat(GameUtilities.PlayerPrefs.SFXVolume, _sfxVolume);

                OnSFXVolumeChange?.Invoke(_sfxVolume);
            }
        }

        private float _musicVolume;
        public float MusicVolume
        {
            get
            {
                return _musicVolume;
            }
            set
            {
                _musicVolume = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat(GameUtilities.PlayerPrefs.MusicVolume, _musicVolume);

                OnMusicVolumeChange?.Invoke(_musicVolume);
            }
        }

        //
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _isSFXMuted = PlayerPrefs.GetInt(GameUtilities.PlayerPrefs.isSFXMuted, 0) == 1;
            _isMusicMuted = PlayerPrefs.GetInt(GameUtilities.PlayerPrefs.isMusicMuted, 0) == 1;
            _sfxVolume = PlayerPrefs.GetFloat(GameUtilities.PlayerPrefs.SFXVolume, 1);
            _musicVolume = PlayerPrefs.GetFloat(GameUtilities.PlayerPrefs.MusicVolume, 1);
        }

        public void PlayMusic(AudioClip _music)
        {
            if (IsMusicMuted)
                return;

            musicSource.Stop();
            musicSource.clip = _music;
            musicSource.Play();
        }

        public static float ConvertFloatToDB(float _value)
        {
            if (_value == 0)
                return -144.0f;
            return 20 * Mathf.Log10(_value);
        }

        public static float ConvertDBToFloat(float _value)
        {
            return Mathf.Pow(10.0f, _value/20.0f);
        }
    }
}
