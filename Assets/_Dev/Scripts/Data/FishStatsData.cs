using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class FishStatsData
{
    public FishState State;
    public float Stamina;
    public float ConsumeStamina;
    public float TimeToKnockDown;
    public float Speed;
    public float Sprint; // percent ดีมั้ย?
    public float RegenRate;
    public bool IsCatch = false;
    public bool IsStun = false;
    
    public float Power = 1;
    [Header("Percent Of pulling")]
    public float PercentPulling = .5f;
    public bool IsPulling = false;
    [Header("PullCountdown")]
    public float PullCountdown;
    public float PullCountdownMin;
    public float PullCountdownMax;
}
