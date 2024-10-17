using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Sirenix.OdinInspector;
using System;
using System.Resources;
using Unity.VisualScripting;
public class Fish : MonoBehaviour
{

    [Header("MovingComponent")]
    [SerializeField]ToTargetMovement _toTargetMovement;
    [SerializeField]RandomMovement _randomMovement;
    [SerializeField]FleeMovement _fleeMovement;
    [SerializeField]Transform _mouseTransform;
    public Bait _bait { get; private set; }
    
    public float Rand;
    public float Power;
    [Header("Stats & Data")]
    [SerializeField] FishStatsConfig FishStatsConfig;
    public FishStatsData StatsData = new FishStatsData();
    [ShowInInspector]public float SpeedTotal { get; private set; }
    [ShowInInspector] public float DiffSpeed { get; private set; }
    [ShowInInspector] public float DiffPower { get; private set; }
    [ShowInInspector] public float DiffPercent { get; private set; }
    [ShowInInspector] float _randomPercentPulling;

    [Header("Debug")]
    [SerializeField] bool _isDebug;
    [SerializeField] FishDebugUI _fishDebugUIPrefab;

    Vector2 _stopPosition;

    //Observable
    IDisposable TimeCountdownPullingObservable;

    IDisposable TimeToStunningObservable;
    private void Start()
    {
        this.ObserveEveryValueChanged(_ => _.FishStatsConfig).Subscribe(fishStatsConfig =>
        {
            
            RefreshFishStats();
        }).AddTo(this);
        AddListener();
        if(_isDebug)
        {
            var fishDebugUI = Instantiate(_fishDebugUIPrefab);
            fishDebugUI.SetFish(this);
        }
    }
    void AddListener()
    {
        this.StatsData.ObserveEveryValueChanged(_ =>_.Stamina).Where(stamina => stamina <= 0).Subscribe(stamina => {
            TimeCountdownPullingObservable?.Dispose();
            StatsData.IsPulling = false;
        }).AddTo(this);
        Observable.Interval(TimeSpan.FromSeconds(StatsData.RoundPullingDuration)).Subscribe(_ =>
        {
            CalculatePulling();
        }).AddTo(this);
        _toTargetMovement.OnBaitHasGone.Subscribe(_ =>
        {
            EnableRandomMovement();
        }).AddTo(this);
    }
    void RefreshFishStats()
    {
        StatsData.Stamina = FishStatsConfig.FishStatsData.Stamina;
        StatsData.ConsumeStamina = FishStatsConfig.FishStatsData.ConsumeStamina;
        StatsData.RegenStamina = FishStatsConfig.FishStatsData.RegenStamina;
        StatsData.IsRandomStamina = FishStatsConfig.FishStatsData.IsRandomStamina;
        StatsData.MinRegenStamina = FishStatsConfig.FishStatsData.MinRegenStamina;
        StatsData.MaxRegenStamina = FishStatsConfig.FishStatsData.MaxRegenStamina;
       

        StatsData.Power = FishStatsConfig.FishStatsData.Power;
        StatsData.CurrentPower = FishStatsConfig.FishStatsData.CurrentPower;

        StatsData.TimeToKnockDown = FishStatsConfig.FishStatsData.TimeToKnockDown;
        StatsData.RegenRate = FishStatsConfig.FishStatsData.RegenRate;
        StatsData.State = FishStatsConfig.FishStatsData.State;
        StatsData.Speed = FishStatsConfig.FishStatsData.Speed;
        StatsData.Sprint = FishStatsConfig.FishStatsData.Sprint;

        StatsData.RoundPullingDuration = FishStatsConfig.FishStatsData.RoundPullingDuration;
        StatsData.PercentPulling = FishStatsConfig.FishStatsData.PercentPulling;
        StatsData.IsPulling = FishStatsConfig.FishStatsData.IsPulling;
        StatsData.IgnorePullingWhenStaminaLower = FishStatsConfig.FishStatsData.IgnorePullingWhenStaminaLower;

        StatsData.PullCountdown = FishStatsConfig.FishStatsData.PullCountdown;
        StatsData.PullCountdownMin = FishStatsConfig.FishStatsData.PullCountdownMin;
        StatsData.PullCountdownMax = FishStatsConfig.FishStatsData.PullCountdownMax;

        StatsData.DistanceToWandering = FishStatsConfig.FishStatsData.DistanceToWandering;
        StatsData.TimeWanderingMin = FishStatsConfig.FishStatsData.TimeWanderingMin;
        StatsData.TimeWanderingMax = FishStatsConfig.FishStatsData.TimeWanderingMax;
        StatsData.RateToInteresting = FishStatsConfig.FishStatsData.RateToInteresting;
        StatsData.RateToEatBait = FishStatsConfig.FishStatsData.RateToEatBait;
    }
    public void CatchBait(Bait bait)
    {
        _bait = bait;
    }
    private void Update()
    {
        
        CalculatePower();
        if (_bait != null)
        {   
            switch(StatsData.State)
            {
                case FishState.Flee:
                    Fighting();
                    break;
                case FishState.Stunning:
                    Stunning();
                    break;
            }
        }
    }
    void Stunning()
    {
        DiffSpeed = 0;
        _bait.Rod.DiffSpeed = 0;
    }
    void Fighting()
    {
        DiffPower = StatsData.CurrentPower - _bait.Rod.StatsData.PullPower;
        DiffPercent = StatsData.CurrentPower / StatsData.Power;
        DiffSpeed = StatsData.Sprint * DiffPercent;
        if (StatsData.IsPulling)
        {
            _bait.Rod.DiffSpeed = _bait.Rod.StatsData.DragSpeed * (1 - DiffPercent);
        }
        else
        {
            _bait.Rod.DiffSpeed = _bait.Rod.StatsData.DragSpeed;
        }

        if (DiffPower < 0 || !StatsData.IsPulling || StatsData.State == FishState.Stuned)
        {
            transform.position = _bait.transform.position;
        }
        else
        {
            if (StatsData.IsPulling)
            {
                _bait.transform.position = transform.position;
                GetComponent<RotationDirection>().SetupTargetPosition(_bait.transform.position);
            }
        }
        CalculateStamina();
    }
    void CalculatePulling()
    {
        if (StatsData.IsPulling) return;
        if (StatsData.Stamina <= StatsData.IgnorePullingWhenStaminaLower) return;
        _randomPercentPulling = UnityEngine.Random.Range(0, 1f);
        if(_randomPercentPulling > StatsData.PercentPulling)
        {
            
            StatsData.IsPulling = true;
            CreatePullingCountdown();
        }
    }
    void CreatePullingCountdown()
    {
        TimeCountdownPullingObservable?.Dispose();
        var timeRandom = UnityEngine.Random.Range(StatsData.PullCountdownMin, StatsData.PullCountdownMax);
        TimeCountdownPullingObservable = Observable.Timer(TimeSpan.FromSeconds(timeRandom)).Subscribe(_ =>
        {
            StatsData.IsPulling = false;
        }).AddTo(this);
    }
    void CalculatePower()
    {
        StatsData.CurrentPower = StatsData.Power * StatsData.Stamina * 0.01f;
    }
    private void CalculateStamina()
    {
        if(StatsData.IsPulling)
        {
            SpeedTotal = StatsData.Speed + (StatsData.Sprint * StatsData.Stamina);
            StatsData.Stamina -= StatsData.ConsumeStamina;
            if (StatsData.Stamina <= 0) StatsData.Stamina = 0;
        }
        else
        {
            if(StatsData.IsRandomStamina)
            {
                StatsData.RegenStamina = UnityEngine.Random.Range(StatsData.MinRegenStamina,StatsData.MaxRegenStamina);
            }
            StatsData.Stamina += StatsData.RegenStamina;
            if (StatsData.Stamina >= 100) StatsData.Stamina = 100;
        }
        
    }
    public void Shock()
    {
        TimeToStunningObservable?.Dispose();
        StatsData.IsPulling = false;
        StatsData.State = FishState.Stunning;
        _stopPosition = transform.position;
        TimeToStunningObservable = Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            StatsData.State = FishState.Flee;
            StatsData.IsPulling = true;

        }).AddTo(this);
       
        
    }
    public void TakeDamage(int damage)
    {
        if (StatsData.State == FishState.Stunning) return;
        Shock();
        StatsData.Stamina -= damage;
        StartCoroutine(ResourcesLoaderManager.Instance.GetObject<TextPopup>(PathKey.DamagePopup, textPopup => {
            
            var popup = Instantiate(textPopup,GameObject.Find("Panel").transform);
            var text = popup.GetComponent<TextPopup>();
            text.SetRectPosition(transform.position);
            text.SetText("Shock! "+damage.ToString());
        }));
    }
    public void ReleaseBait()
    {
        EnableRandomMovement();
        RefreshFishStats();
        

    }
    void GetBait()
    {

    }
    void Wandering()
    {
        StatsData.State = FishState.Wandering;

    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag(TagKey.BaitAura) && StatsData.State == FishState.Moving)
        {
            var bait = collision.gameObject.GetComponentInParent<Bait>();
            if (bait != null && !bait.HasFish)
            {
                EnableTotargetMovement(bait);
            }

        }

        if (collision.CompareTag(TagKey.Bait) && StatsData.State == FishState.Eating)
        {
            if (collision.gameObject.TryGetComponent(out Bait bait) && !bait.HasFish)
            {
                bait.GetFish(this);
                CatchBait(bait);
                EnableFleeMovement();
            }
        }
    }
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
        
    //    if(collision.CompareTag(TagKey.BaitAura) && StatsData.State == FishState.Moving)
    //    {
    //        var bait = collision.gameObject.GetComponentInParent<Bait>();
    //        if (bait != null && !bait.HasFish)
    //        {
    //            EnableTotargetMovement(bait);
    //        }
            
    //    }

    //    if (collision.CompareTag(TagKey.Bait) && StatsData.State == FishState.Eating)
    //    {
    //        if(collision.gameObject.TryGetComponent(out Bait bait) && !bait.HasFish)
    //        {
    //            bait.GetFish(this);
    //            CatchBait(bait);
    //            EnableFleeMovement();
    //        }
    //    }
    //}
    void EnableRandomMovement()
    {
        StatsData.State = FishState.Moving;
        _randomMovement.enabled = true;
        _fleeMovement.Dispose();
        _toTargetMovement.Dispose();
    }
    void EnableFleeMovement()
    {
        StatsData.State = FishState.Flee;
        _fleeMovement.enabled = true;
        _fleeMovement.BaitPosition = _bait.BaitStart;

        _randomMovement.Dispose();
        _toTargetMovement.Dispose();
    }
    void EnableTotargetMovement(Bait bait)
    {
        Debug.Log("EnableTargetmovement");
        StatsData.State = FishState.Interesting;
        _toTargetMovement.enabled = true;
        _toTargetMovement.SetupBait(bait);
        
        

        _fleeMovement.Dispose();
        _randomMovement.Dispose();
    }
    void ChangeMovement(FishState state)
    {
        switch(state)
        {
            case FishState.Wandering:
                break;
            
        }
    }
}