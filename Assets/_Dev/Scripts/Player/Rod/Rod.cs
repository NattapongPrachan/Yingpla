using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
[RequireComponent(typeof(RodUI))]
public class Rod : MonoBehaviour
{
    
    [SerializeField] PlayerTouchScreen _playerTouchScreen;
    [SerializeField] RodUI _rodUI;
    [Header("Bait")]
    [SerializeField] Bait _baitPrefab;
    [SerializeField] Image _spawnPoint;
    
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 end;
    Vector2 _screenPoint = Vector2.zero;
    [SerializeField]Bait _baitObject;

    [Header("State")]
    public RodState RodState;
    public bool IsPulling;
    [Header("Config")]
    [SerializeField] float _castingSpeed = 2f;
    [SerializeField] float _dragSpeed = 2f;
    [SerializeField] float _reachToRodDistance = 1f;
    private void Start()
    {
        _playerTouchScreen.OnScreenPoint.Subscribe(screenPoint =>
        {
            switch(RodState)
            {
                case RodState.None:
                    CastingBait(screenPoint);
                    break;
            }
        }).AddTo(this);
        _playerTouchScreen.OnPressed.Subscribe(isPressed => {
            IsPulling = isPressed;
        }).AddTo(this);
    }
    public void CastingBait(Vector2 screenPoint)
    {
        _screenPoint = screenPoint;
        var direction = screenPoint - new Vector2(_spawnPoint.transform.position.x, _spawnPoint.transform.position.y);
        _spawnPoint.transform.rotation = Quaternion.Euler(0, 0, GameUtils.CalculateAngleFromDirection(direction));
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_spawnPoint.rectTransform, _spawnPoint.transform.position, Camera.main, out start);
        end = Camera.main.ScreenToWorldPoint(_screenPoint);
        end.z = 0f;
        RodState = RodState.Casting;
        _baitObject = Instantiate(_baitPrefab, start, Quaternion.identity);
        _baitObject.ObserveEveryValueChanged(_ => _.HasFish).Subscribe(hasFish =>
        {
            if(hasFish)
            {
                RodState = RodState.Staying;
            }
        }).AddTo(this);
        _baitObject.transform.DOMove(end, _castingSpeed).OnComplete(() => {
            RodState = RodState.Staying;
        });
        _rodUI.SetupBaitObject(_baitObject.transform, start);
    }
    void Pulling()
    {
        if (!IsPulling) return;
        if(RodState == RodState.Draging || RodState == RodState.Staying)
        {
            var direction = start - _baitObject.transform.position;
            direction.z = 0;
            var distance = Vector3.Distance(_baitObject.transform.position, start);
            if (distance <= _reachToRodDistance)
            {
                DestroyBait();
            }
            else
            {
                _baitObject.transform.position += direction.normalized * _dragSpeed * Time.deltaTime;
            }
 
        }
        
    }
    [Button]
    public void DestroyBait()
    {
        _baitObject.Dispose();
        _baitObject = null;
        RodState = RodState.None;
        _rodUI.Reset();
    }

    private void Update()
    {
        Pulling();
    }
}
