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
        var rodToWorldPoint = Camera.main.ScreenToWorldPoint(_rodImage.transform.position);
        var direction = _baitTransform.position - rodToWorldPoint;
        var angle = GameUtils.CalculateAngleFromDirection(direction);
        _rodImage.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void UpdateLine()
    {
        if (_baitTransform == null) return;
        Vector3[] positions = new Vector3[_lineSegment + 1];
        for (int i = 0; i <= _lineSegment; i++)
        {
            float t = (float)i / _lineSegment;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_rodImage.rectTransform, _spawnPoint.transform.position, Camera.main, out Vector3 spawnPointToWorldPoint);
            Vector3 point = Vector3.Lerp(spawnPointToWorldPoint, _baitTransform.position, t);
            if (i > 0)
                point.y += Mathf.Sin((Time.time + t) * _swaySpeed) * _swayAmount;
            positions[i] = point;
        }

        _lineRenderer.SetPositions(positions);
    }
}
