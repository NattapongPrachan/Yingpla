using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using static UnityEngine.Rendering.HableCurve;
using System.Net;
using UnityEngine.Animations.Rigging;
using System;
public class RodUI : MonoBehaviour
{
    [Header("Rod Image")]
    [SerializeField] Image _rodImage;
    [Header("LineBaitAnimation")]
    [SerializeField] Transform _spawnPoint;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] float _swayAmount = 0.2f;
    [SerializeField] float _swaySpeed = 2f; // ความเร็วในการย้วย
    [SerializeField] int _lineSegment = 10;
    public Rod Rod;
    Vector3 _baitStartPosition;
    Transform _baitTransform;
   
    void Start()
    {
        _lineRenderer.positionCount = _lineSegment+1;
    }
    public void SetupBaitObject(Transform baitTransform,Vector3 baitStartPosition)
    {
        _baitTransform = baitTransform;
        _baitStartPosition = baitStartPosition;
    }
    public void Reset()
    {
        _baitTransform = null;
        Vector3[] positions = new Vector3[_lineSegment + 1];
        for (int i = 0; i < _lineSegment; i++)
        {
            positions[i] = Vector3.zero;
        }
        _lineRenderer.SetPositions(positions);
    }
    void Update()
    {
        UpdateLine();
        UpdateRodUIRotation();
    }

    private void UpdateRodUIRotation()
    {
        if (_baitTransform == null) return;
        var baitScreenPoint = Camera.main.WorldToScreenPoint(_baitTransform.position);
        var direction = baitScreenPoint - _spawnPoint.transform.position;
        var angle = GameUtils.CalculateAngleFromDirection2d(Rod.StatsData.InputDirection);
        
        _rodImage.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void UpdateLine()
    {
        if (_baitTransform == null) return;
        Vector3[] positions = new Vector3[_lineSegment + 1];
        for (int i = 0; i <= _lineSegment; i++)
        {
            float t = (float)i / _lineSegment;
            var spawnPointToWorldPoint = Camera.main.ScreenToWorldPoint(_spawnPoint.transform.position);
            //spawnPointToWorldPoint.y = GameUtils.YAxis;
            var endPosition = new Vector3(_baitTransform.position.x, spawnPointToWorldPoint.y, _baitTransform.position.z);
            Vector3 point = Vector3.Lerp(spawnPointToWorldPoint, _baitTransform.position, t);
            if (i > 0)
                point.z += Mathf.Sin((Time.time + t) * _swaySpeed) * _swayAmount;
            positions[i] = point;
        }

        _lineRenderer.SetPositions(positions);
    }
}
