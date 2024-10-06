using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using UniRx;
using static UnityEngine.UIElements.UxmlAttributeDescription;
public class PlayerTouchScreen : MonoBehaviour
{
    public static Subject<Vector2> OnScreenPoint = new Subject<Vector2>();
    public static Subject<Vector2> OnSelectWorldPoint = new Subject<Vector2>();
    private void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 point = Mouse.current.position.ReadValue();
            point.z = 10;
            var worldPoint = Camera.main.ScreenToWorldPoint(point);
            OnScreenPoint.OnNext(point);
            OnSelectWorldPoint.OnNext(worldPoint);
        }
    }
}
