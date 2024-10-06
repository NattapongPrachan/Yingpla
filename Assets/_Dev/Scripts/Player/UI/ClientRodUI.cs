using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using static UnityEngine.Rendering.HableCurve;
using System.Net;
using UnityEngine.Animations.Rigging;
public class ClientRodUI : MonoBehaviour
{
    [SerializeField] GameObject _baitPrefab;
    [SerializeField] Image _spawnPoint;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] float _baitSpeed = 2f;
    [SerializeField] float _swayAmount = 0.2f;
    [SerializeField] float _swaySpeed = 2f; // ความเร็วในการย้วย
    [SerializeField] int _lineSegment = 10;
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 end;
    Vector2 _screenPoint = Vector2.zero;
    GameObject _baitObject;
   
    void Start()
    {
        _lineRenderer.positionCount = _lineSegment+1;
        PlayerTouchScreen.OnScreenPoint.Subscribe(screenPoint =>
        {
            _screenPoint = screenPoint;
            var direction = screenPoint - new Vector2(_spawnPoint.transform.position.x,_spawnPoint.transform.position.y);
            _spawnPoint.transform.rotation = Quaternion.Euler(0, 0, GameUtils.CalculateAngleFromDirection(direction));
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_spawnPoint.rectTransform, _spawnPoint.transform.position, Camera.main, out start);
            end = Camera.main.ScreenToWorldPoint(_screenPoint);
            end.z = -0.1f;
            _baitObject = Instantiate(_baitPrefab,start,Quaternion.identity);
            _baitObject.transform.DOMove(end, _baitSpeed);
        }).AddTo(this);
    }
    void Update()
    {
        UpdateLine();
    }
    void UpdateLine()
    {
        if (_baitObject == null) return;
        Vector3[] positions = new Vector3[_lineSegment + 1];
        for (int i = 0; i <= _lineSegment; i++)
        {
            float t = (float)i / _lineSegment;
            Vector3 point = Vector3.Lerp(start, _baitObject.transform.position, t);
            if(i > 0)
                point.y += Mathf.Sin((Time.time + t) * _swaySpeed) * _swayAmount;
            positions[i] = point;
        }

        _lineRenderer.SetPositions(positions);
    }
}
