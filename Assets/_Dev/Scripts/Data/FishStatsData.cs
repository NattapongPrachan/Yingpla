using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class FishStatsData
{
    public FishState State;
    [Header("Power")]
    public float Power = 1;
    public float CurrentPower;
    [Header("Stamina")]
    public float Stamina;
    public float ConsumeStamina =0.1f;
    public float RegenStamina = 0.25f;
    public bool IsRandomStamina = false;
    public float MinRegenStamina = 0.01f;
    public float MaxRegenStamina = 0.05f;
    public float TimeToKnockDown;
    public float Speed;
    public float Sprint; // percent ดีมั้ย?
    public float RegenRate;
    public bool IsCatch = false;
    public bool IsStun = false;
    [Header("Percent Of pulling")]
    public float RoundPullingDuration = 2;
    public float PercentPulling = .5f;
    public float IgnorePullingWhenStaminaLower = 50;
    public bool IsPulling = false;
    [Header("PullCountdown")]
    public float PullCountdown;
    public float PullCountdownMin;
    public float PullCountdownMax;
    [Header("Stun")]
    public float RegenStun;
    [Header("Wandering To Target")]
    public float DistanceToWandering;
    public float TimeWanderingMin;
    public float TimeWanderingMax;
    public float RateToInteresting;
    public float RateToEatBait;
    [SerializeField]
    public Vector2 MoveDirection;
    
}
