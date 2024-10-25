using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AssetScene
{
    public Dictionary<string,object> _pathCollection = new Dictionary<string, object>();
    public string toSceneName;

    public void Dispose()
    {
        _pathCollection.Clear();
    }
    public virtual void PrepareData(System.Action Callback){}

}

public class TestAssetScene : AssetScene
{
    public override void PrepareData(Action Callback)
    {
        toSceneName = "Assets/Scenes/TestingScene.unity";
        _pathCollection.Add("FemaleDummy","Assets/Prefab/FemaleDummy.prefab");
        Callback();
    }
}
