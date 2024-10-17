using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UniRx;
using UnityEngine.UI;
using System.Drawing;
public class PlayerTouchScreen : MonoBehaviour
{
    public Subject<Vector3> OnScreenPoint = new Subject<Vector3>();
    public static Subject<Vector3> OnSelectWorldPoint = new Subject<Vector3>();
    public Subject<bool> OnPressed = new Subject<bool>();
    public Subject<bool> OnShock = new Subject<bool>();
    [SerializeField] Vector3 WorldPoint;
    [SerializeField] Image Image;
    [SerializeField] GameObject targetMover;
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
            //point.y = 0;
            var worldPoint = Camera.main.ScreenToWorldPoint(point);
            WorldPoint = worldPoint;
            
            worldPoint.y = 0;

            //Image.transform.position = point;
            //targetMover.transform.position = worldPoint;


            OnScreenPoint.OnNext(point);
            OnSelectWorldPoint.OnNext(worldPoint);
        }
        if(Mouse.current.rightButton.wasPressedThisFrame)
        {
            OnShock.OnNext(true);
        }
    }
}
