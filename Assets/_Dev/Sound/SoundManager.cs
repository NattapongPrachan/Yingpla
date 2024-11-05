using DG.Tweening;
using Mono.Cecil;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField]AssetLoaderData _loaderData;
    [SerializeField] float _masterVolume;
    [SerializeField] float _musicVolume;
    [SerializeField] float _sfxVolume;

    AudioSource _musicSource;
    bool _isPlaying = false;
    [SerializeField] bool _isMusic = false;
    [SerializeField] bool _playAll = false;
    public override void Init()
    {
        base.Init();
        
    }
    [Button]
    public void Play()
    {
        _isPlaying = !_isPlaying;
    }
    [Button]
    public void PlayMusic()
    {
        PlayMusic(MusicKey.Songran);
    }
    private async void Start()
    {
        AssetLoaderHandle.Instance.AddPreloadData(_loaderData);
        await AssetLoaderHandle.Instance.Load();
        ObjectPoolManager.Instance.CreateAssetToPool(AssetLoaderHandle.Get<GameObject>(PrefabKey.AudioSource));
    }
    private void Update()
    {
        if(_playAll)
        {
            PlayMusic(MusicKey.Songran);
            PlayOneShot(SFXKey.Jump);
            return;
        }
        if(_isPlaying)
        {
            if(_isMusic)
            {
                PlayMusic(MusicKey.Songran,Vector3.zero);
            }
            else
            {
                PlayOneShot(SFXKey.Jump);
            }   
        }
    }
    public static void PlayOneShot(string name,Vector3 position = default)
    {
        PlaySound(name,position,false, Instance._sfxVolume);
    }
    public static void PlayMusic(string name, Vector3 position = default)
    {
        PlaySound(name,default, true, Instance._musicVolume);
    }
    public static void PlaySound(string soundName , Vector3 position, bool loop,float volume)
    {
        var audioGameObject = ObjectPoolManager.Instance.Get(PrefabKey.AudioSource, ObjectPoolManager.RootName.Sound);
        var audioClip = AssetLoaderHandle.Get<AudioClip>(soundName);
        var audioSource = (AudioSource)audioGameObject.GetComponent("AudioSource");
        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.volume = volume;
        if (audioSource.loop)
        {
            if (Instance._musicSource != null)
            {
                var tempSource = Instance._musicSource;
                Instance._musicSource.DOFade(0, 0.25f).OnComplete(() =>
                {
                    tempSource.Stop();
                    tempSource = null;
                });
            }

            Instance._musicSource = audioSource;
            Instance._musicSource.Play();
            Instance._musicSource.transform.position = position;
        }
        else
        {
            audioSource.PlayOneShot(audioClip);
            audioSource.transform.position = position;
        }
    }
}
