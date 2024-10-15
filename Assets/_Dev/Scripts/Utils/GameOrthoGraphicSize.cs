using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(9)]
public class GameOrthoGraphicSize : MonoInstance<GameOrthoGraphicSize>
{
    [SerializeField] CinemachineVirtualCamera _virtualCamera;
    [SerializeField] float _size;
    public Vector2 BaseScreenSize = new Vector2(1920, 1080);
    public float pixelsPerUnit = 100f; // ค่า Pixels Per Unit ของ sprite
    private void Start()
    {
        _size = BaseScreenSize.y / (2 * pixelsPerUnit);
        _virtualCamera.m_Lens.OrthographicSize = _size;
    }
}
