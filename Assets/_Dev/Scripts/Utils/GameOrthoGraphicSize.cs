using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(9)]
public class GameOrthoGraphicSize : MonoInstance<GameOrthoGraphicSize>
{
    [SerializeField] CinemachineVirtualCamera _virtualCamera;
    [SerializeField] float _size;
    public float screenHeight = 1080f; // ความสูงที่คุณต้องการ
    public float pixelsPerUnit = 100f; // ค่า Pixels Per Unit ของ sprite
    private void Start()
    {
        _size = Screen.height / (2 * pixelsPerUnit);
        _virtualCamera.m_Lens.OrthographicSize = _size;
    }
}
