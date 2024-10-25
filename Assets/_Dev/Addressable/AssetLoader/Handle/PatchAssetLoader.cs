using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatchAssetLoader : AssetLoader
{
    public PatchAssetLoader()
    {
        Labels = new List<string>() {"Patch" };
    }
}
