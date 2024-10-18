using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationDirection : MonoBehaviour
{
    [SerializeField] Transform _rotTransform;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] float _rotateDuration;
    public void SetupTargetPosition(Vector3 targetPosition)
    {
        var direction = targetPosition - transform.position;
        var angle = GameUtils.CalculateAngleFromDirection(direction);
        var rot = Quaternion.Euler(0, angle, 0);
        UpdateRotaiton(rot);
       // _rotTransform.transform.DORotate(rot.eulerAngles, _rotateDuration);

    }
    public void UpdateRotaiton(Quaternion newRotation)
    {
        _rotTransform.DORotateQuaternion(newRotation, _rotateDuration);
    }
    private void Update()
    {
        
    }

}
