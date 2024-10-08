using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new FishStatsData", menuName = "scriptableObject/FishStatsData", order = 100)]
public class FishStatsData : ScriptableObject
{
    public float Stamina;
    public float TimeToKnockDown;
    public float Speed;
    public float RegenRate;
    public bool IsCatch = false;
    public bool IsStun = false;
    public float Power = 1;
    public FishState State;
}
