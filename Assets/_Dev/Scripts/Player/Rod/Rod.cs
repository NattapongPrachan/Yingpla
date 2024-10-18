using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using System;
[RequireComponent(typeof(RodUI))]
public class Rod : SerializedMonoBehaviour
{
    
    [SerializeField] PlayerTouchScreen _playerTouchScreen;
    [SerializeField] RodUI _rodUI;
    [Header("Bait")]
    [SerializeField] Bait _baitPrefab;
    [SerializeField] Image _spawnPoint;
    [Header("Input&Controller")]
    [SerializeField]RodInput _playerInput;
    [SerializeField] Vector3 start;
    [SerializeField] Vector3 end;
    Vector2 _screenPoint = Vector2.zero;
    [SerializeField]Bait _baitObject;
    [Header("FishingLine")]
    [SerializeField] Slider _fishingLineSlider;
    [Header("Config")]
    public RodConfig Config;
    public RodStatsData StatsData;
    public float DiffSpeed;

    [SerializeField] float fishAngle;
    [SerializeField] float rodAngle;
    [SerializeField] float fishToRodAngle;
    [SerializeField] float baitAngle;
    private void Start()
    {
        UpdateRodStatsData();
        AddInput();
        _playerTouchScreen.OnScreenPoint.Subscribe(screenPoint =>
        {
            switch(StatsData.RodState)
            {
                case RodState.None:
                    CastingBait(screenPoint);
                    break;
            }
        }).AddTo(this);
        _playerTouchScreen.OnPressed.Subscribe(isPressed => {
            StatsData.IsPulling = isPressed;
        }).AddTo(this);
        _playerTouchScreen.OnShock.Subscribe(isShock =>
        {
            _baitObject?.Shock(50);
        }).AddTo(this);
        _rodUI.Rod = this;
    }
    void AddInput()
    {
        Debug.Log("input " + _playerInput.Player.Movement);
        _playerInput = new RodInput();
        _playerInput.Enable();
        _playerInput.Player.Movement.performed += Movement_performed;
    }

    private void Movement_performed(InputAction.CallbackContext obj)
    {
        StatsData.InputDirection = obj.ReadValue<Vector2>();
    }

    public void UpdateRodStatsData()
    {
        StatsData = new RodStatsData();
        StatsData.CastingSpeed = Config.RodStatsData.CastingSpeed;
        StatsData.DragSpeed = Config.RodStatsData.DragSpeed;
        StatsData.PullPower = Config.RodStatsData.PullPower;
        StatsData.ReachToDistance = Config.RodStatsData.ReachToDistance;
       
        StatsData.IsPulling = Config.RodStatsData.IsPulling;

        StatsData.DiffAngleRate = Config.RodStatsData.DiffAngleRate;
        StatsData.LineStrength = Config.RodStatsData.LineStrength;
        StatsData.CurrentLineStrength = 0;
        StatsData.IncreaseStrength = Config.RodStatsData.IncreaseStrength;
        StatsData.DecreaseStrength = Config.RodStatsData.DecreaseStrength;
        //--//
        DiffSpeed = StatsData.DragSpeed;
        StatsData.InputDirection = Vector2.zero;
        StatsData.CurrentDiffAngle = 0;
        
       
    }
    public void CastingBait(Vector3 screenPoint)
    {
        _screenPoint = screenPoint;
        var direction = screenPoint - _spawnPoint.transform.position;
        _spawnPoint.transform.rotation = Quaternion.Euler(0, 0, GameUtils.CalculateAngleFromDirection(direction));
        RectTransformUtility.ScreenPointToWorldPointInRectangle(_spawnPoint.rectTransform, _spawnPoint.transform.position, Camera.main, out start);
        start = Camera.main.ScreenToWorldPoint(_spawnPoint.transform.position);
        start.y = GameUtils.YAxis;
        end = Camera.main.ScreenToWorldPoint(_screenPoint);
        end.y = GameUtils.YAxis;
        StatsData.RodState = RodState.Casting;
        _baitObject = Instantiate(_baitPrefab, start, Quaternion.identity);
        _baitObject.Rod = this;
        _baitObject.BaitStart = start;
        _baitObject.ObserveEveryValueChanged(_ => _.HasFish).Subscribe(hasFish =>
        {
            if(hasFish)
            {
                StatsData.RodState = RodState.Staying;
            }
        }).AddTo(this);
        var distance = Vector3.Distance(end, start);
        var castingTime = GameUtils.CalculateDistanceSpeedToTime(StatsData.CastingSpeed,distance );
        _baitObject.transform.DOMove(end, castingTime).OnComplete(() => {
            StatsData.RodState = RodState.Staying;
            _baitObject.OpenAuraCollider();
        });
        _rodUI.SetupBaitObject(_baitObject.transform, start);
    }
    void UpdateFishingLine()
    {
        if (_baitObject == null ||_baitObject.Fish == null) return;

        var fishToRodDirection = new Vector3(_rodUI.WorldTransform.x,0,_rodUI.WorldTransform.z) - _baitObject.transform.position;
        var direction = new Vector2(_rodUI.WorldTransform.x, _rodUI.WorldTransform.z) - new Vector2(_baitObject.transform.position.x, _baitObject.transform.position.z);
        fishToRodAngle = GameUtils.CalculateAngleFromDirection2d(direction, true); 
        rodAngle = GameUtils.CalculateAngleFromDirection2d(StatsData.InputDirection,true);
        baitAngle = _rodUI.GetRodToBaitAngle;

        StatsData.CurrentDiffAngle = Mathf.Abs(_rodUI.GetRodToBaitAngle - (rodAngle+180));
        if(StatsData.CurrentDiffAngle > StatsData.DiffAngleRate)
        {
            StatsData.CurrentLineStrength += StatsData.IncreaseStrength;
            if (StatsData.CurrentLineStrength >= StatsData.LineStrength) StatsData.CurrentLineStrength = StatsData.LineStrength;
        }
        else
        {
            StatsData.CurrentLineStrength -= StatsData.DecreaseStrength;
            if (StatsData.CurrentLineStrength <= 0) StatsData.CurrentLineStrength = 0;
        }
        
        if (StatsData.CurrentLineStrength >= StatsData.LineStrength)
        {
            //DestroyBait();
        }
        _fishingLineSlider.value = (StatsData.CurrentLineStrength / StatsData.LineStrength);
    }
    void Pulling()
    {
        if (!StatsData.IsPulling) return;
        if(StatsData.RodState == RodState.Draging || StatsData.RodState == RodState.Staying || StatsData.RodState == RodState.None)
        {
            var direction = start - _baitObject.transform.position;
            var distance = Vector3.Distance(_baitObject.transform.position, start);
            if (distance <= StatsData.ReachToDistance)
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
        _rodUI.Reset();
        Reset();
        ResetUI();
    }
    private void Reset()
    {
        UpdateRodStatsData();
    }
    void ResetUI()
    {
        _fishingLineSlider.value = 0;
    }

    private void Update()
    {
        Pulling();
        UpdateFishingLine();
    }
}
