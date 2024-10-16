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
    
}
public struct PathKey
{
    public static readonly string DamagePopup = "Popup/DamagePopup";
}
