using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestAssetLoader : AssetLoader
{
    public TestAssetLoader()
    {
        Labels = new List<string>() { "Login", "default" };
    }
}
