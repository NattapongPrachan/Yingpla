using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(7)]
public static class GameUtils 
{
    public static float CalculateDistanceSpeedToTime(float speed , float distance)
    {
        return  distance / speed;
    }
    public static float CalculateAngleFromDirection(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360; 
        //angle -= 90;
        return angle;
    }
   
}
