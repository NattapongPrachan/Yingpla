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
    public RodConfig Config;
    public float DiffSpeed;
    private void Start()
    {
        DiffSpeed = Config.DragSpeed;
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
        _playerTouchScreen.OnShock.Subscribe(isShock =>
        {
            _baitObject?.Shock(50);
        }).AddTo(this);
    }
    public void CastingBait(Vector3 screenPoint)
    {
        _screenPoint = screenPoint;
        Debug.Log("screenPoint " + _screenPoint);
       
        //var direction = screenPoint - new Vector2(_spawnPoint.transform.position.x, _spawnPoint.transform.position.y);
        var direction = screenPoint - _spawnPoint.transform.position;
        _spawnPoint.transform.rotation = Quaternion.Euler(0, GameUtils.CalculateAngleFromDirection(direction), 0);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_spawnPoint.rectTransform, _spawnPoint.transform.position, Camera.main, out start);
        var aaa = Camera.main.ScreenToWorldPoint(_spawnPoint.transform.position);
        Debug.Log("aaaa " + aaa);
        start = aaa;
        end = Camera.main.ScreenToWorldPoint(_screenPoint);
        //end.z = 0f;
        RodState = RodState.Casting;
        _baitObject = Instantiate(_baitPrefab, start, Quaternion.identity);
        _baitObject.Rod = this;
        _baitObject.BaitStart = start;
        _baitObject.ObserveEveryValueChanged(_ => _.HasFish).Subscribe(hasFish =>
        {
            if(hasFish)
            {
                RodState = RodState.Staying;
            }
        }).AddTo(this);
        var distance = Vector3.Distance(end, start);
        Debug.Log("Start " + start);
        Debug.Log("end " + end);
        var castingTime = GameUtils.CalculateDistanceSpeedToTime(Config.CastingSpeed,distance );
        _baitObject.transform.DOMove(end, castingTime).OnComplete(() => {
            RodState = RodState.Staying;
            _baitObject.OpenAuraCollider();
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
            if (distance <= Config.ReachToDistance)
            {
                DestroyBait();
            }
            else
            {
                _baitObject.transform.position += direction.normalized * DiffSpeed * Time.deltaTime;
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
