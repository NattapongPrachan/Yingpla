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
    Moving,Interesting,Flee,Pulling,Stunning,Stuned
}
public struct TagKeys
{
    public static readonly string Bait = "Bait";
}
