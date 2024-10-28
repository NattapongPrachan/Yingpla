using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys 
{

}
public enum RodState
{
    None,Casting,Staying,Draging,BaitReach,
}
public enum BaitState
{

}
public enum FishState
{
    Moving,Wandering,Interesting,Eating,Flee,Pulling,Stunning,Stuned
}
public struct TagKey
{
    public static readonly string Bait = "Bait";
    public static readonly string BaitAura = "BaitAura";
}
public struct PrefabKey
{
    public static readonly string AudioSource = "AudioSource";
}
public struct SFXKey
{
    public static readonly string Jump = "Jump";
}
public struct MusicKey
{
    public static readonly string Songran = "Songran";
}
public struct PathKey
{
    public static readonly string DamagePopup = "Popup/DamagePopup";
}

