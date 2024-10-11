using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new RodConfig",menuName = "scriptableObject/RodConfig",order = 10)]
public class RodConfig : ScriptableObject
{
    public float CastingSpeed;
    public float DragSpeed;
    public float PullPower;
    public float ReachToDistance;
}
