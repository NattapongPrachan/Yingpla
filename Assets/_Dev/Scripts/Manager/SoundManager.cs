using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField]AssetLoaderData _loaderData;
    public override async void Init()
    {
        base.Init();
        AssetLoaderHandle.Instance.AddPreloadData(_loaderData);
        await AssetLoaderHandle.Instance.Load();
        
    }
}
