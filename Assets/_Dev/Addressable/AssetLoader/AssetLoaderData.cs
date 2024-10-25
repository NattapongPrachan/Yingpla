using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new AssetLoaderData",menuName = "ScriptableObject/AssetData")]
public class AssetLoaderData : ScriptableObject
{
    public string key;
    public List<string> labels;
}
