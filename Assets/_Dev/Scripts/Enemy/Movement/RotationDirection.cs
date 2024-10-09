using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationDirection : MonoBehaviour
{
    [SerializeField] Transform _rotTransform;
    [SerializeField] SpriteRenderer _spriteRenderer;
    public void SetupTargetPosition(Vector3 targetPosition)
    {
        var direction = targetPosition - transform.position;
        var angle = GameUtils.CalculateAngleFromDirection(direction);
        var rot = Quaternion.Euler(0, 0, angle);
        _rotTransform.transform.DORotate(rot.eulerAngles, 1);

    }
}
