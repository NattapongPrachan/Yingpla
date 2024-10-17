using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class RodStatsData 
{
    public RodState RodState;
    public float CastingSpeed;
    public float DragSpeed;
    public float PullPower;
    public float ReachToDistance;
    
    public bool IsPulling;
    [Header("DifferentAngle")]
    public float CurrentDiffAngle;
    public float DiffAngleRate;
    [Header("FishingLine")]
    public float LineStrength;
    public float CurrentLineStrength;
    public float IncreaseStrength;
    public float DecreaseStrength;

    public Vector2 InputDirection;
}
