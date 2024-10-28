using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
public class AudioListener : MonoBehaviour
{
    public enum AudioState
    {
        Playing,Stopped, Paused
    }
    AudioSource _audioSource;
    ReturnToPool _returnToPool;
    IDisposable _observIsPlaying;
    [SerializeField] bool _isPlaying;
    [SerializeField]AudioState _state = AudioState.Stopped;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _returnToPool = GetComponent<ReturnToPool>();
       
    }
    private void Start()
    {
        _returnToPool = GetComponent<ReturnToPool>();
    }
    void AddListener()
    {
        _observIsPlaying?.Dispose();
        _observIsPlaying = _audioSource.ObserveEveryValueChanged(_ => _.isPlaying).Subscribe(isPlaying => {
            if (_state == AudioState.Playing && !isPlaying)
            {
                Reset();
                _returnToPool.Release();
            }
            _state = isPlaying ? AudioState.Playing : AudioState.Stopped;
        }).AddTo(this);

    }
    private void Reset()
    {
        _state = AudioState.Stopped;
    }
    private void OnEnable()
    {
        AddListener();
    }
    private void OnDisable()
    {
        _observIsPlaying?.Dispose();
    }
}
