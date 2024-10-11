using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
public class PlayerTouchScreen : MonoBehaviour
{
    public Subject<Vector2> OnScreenPoint = new Subject<Vector2>();
    public static Subject<Vector2> OnSelectWorldPoint = new Subject<Vector2>();
    public Subject<bool> OnPressed = new Subject<bool>();
    public Subject<bool> OnShock = new Subject<bool>();
    private void Start()
    {
        Mouse.current.leftButton.ObserveEveryValueChanged(_ => _.isPressed).Subscribe(isPressed => { 
            OnPressed.OnNext(isPressed);
        }).AddTo(this);
    }
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
        if(Mouse.current.rightButton.wasPressedThisFrame)
        {
            OnShock.OnNext(true);
        }
        if(Mouse.current.leftButton.isPressed)
        {
            OnPressed.OnNext(Mouse.current.leftButton.isPressed);
        }
    }
}
