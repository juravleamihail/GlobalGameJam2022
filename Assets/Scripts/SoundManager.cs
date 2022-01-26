using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : Singleton<SoundManager>
    {
        private const string KSfxpath = "Sounds/SFX/";
        private const string KBgMusic = "Sounds/BGMusic/";
            
        [SerializeField] private AudioSource _musicAudioSource;
        [SerializeField] private List<string> _bgMusicNames;
        [SerializeField] private AudioSource _soundAudioSource;
        [SerializeField] private AudioClip[] _swordKillSounds;
        [SerializeField] private AudioClip _winSound;
        public override void Awake()
        {
            base.Awake();
            ChangeRandom();
        }

        [ContextMenu("Change Random")]
        private void ChangeRandom()
        {
            var index = Random.Range(0, _bgMusicNames.Count - 1);
            StartCoroutine(ChangeSong(string.Concat(KBgMusic, _bgMusicNames[index])));
        }

        private IEnumerator ChangeSong(string name)
        {
            var task = Resources.LoadAsync<AudioClip>(name);
            AudioClip oldClip;
            
            if (!_musicAudioSource.isPlaying)
            {
                yield return new WaitUntil(()=> task.isDone);
                
                oldClip = _musicAudioSource.clip;
                _musicAudioSource.clip = task.asset as AudioClip;
                
                _musicAudioSource.Play();
                UnloadClip(oldClip);
                
                yield break;
            }

            while (_musicAudioSource.volume > 0 )
            {
                _musicAudioSource.volume -= Time.deltaTime * .4f;
                yield return null;
            }
            
            yield return new WaitUntil(()=> task.isDone);

            oldClip = _musicAudioSource.clip;
            _musicAudioSource.clip = task.asset as AudioClip;
            _musicAudioSource.volume = 1;
            
            _musicAudioSource.Play();
            UnloadClip(oldClip);
        }

        private void UnloadClip(AudioClip clip)
        {
            Resources.UnloadAsset(clip);
            Resources.UnloadUnusedAssets();
        }
        
        [ContextMenu("Add Selection")]
        private void AddSelection()
        {
            var objs = Selection.objects;
            foreach (var o in objs)
            {
                _bgMusicNames.Add(o.name);
            }
            
            EditorUtility.SetDirty(this);
        }

        public void PlayRandomSwordKillSound(AudioSource playerAudioSource)
        {
            int randomSound = Random.Range(0, _swordKillSounds.Length-1);
        playerAudioSource.clip = _swordKillSounds[randomSound];
        playerAudioSource.Play();
        }

        public void PlayWinSound()
    {
        _soundAudioSource.clip = _winSound;
        _soundAudioSource.Play();
    }
}
