﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(7)]
public static class GameUtils 
{
    public static float YAxis = 1f;
    public static float CalculateDistanceSpeedToTime(float speed , float distance)
    {
        return  distance / speed;
    }
    public static float CalculateAngleFromDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360; 
        return angle;
    }
    public static float CalculateAngleFromDirection2d(Vector3 direction,bool invertAngle = false)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90;
        angle *= invertAngle ? -1 : 1;
        if (angle < 0) angle += 360;
        return angle%360;
    }

}
