using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationDirection : MonoBehaviour
{
    public void SetupTargetPosition(Vector3 targetPosition)
    {
        var direction = targetPosition - transform.position;
        var angle = GameUtils.CalculateAngleFromDirection(direction);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
        
    }
}
