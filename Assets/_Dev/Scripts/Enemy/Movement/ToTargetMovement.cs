using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;
[RequireComponent(typeof(Fish))]
public class ToTargetMovement : MonoBehaviour
{
    public Subject<Unit> OnBaitHasGone = new Subject<Unit>();
    Bait _bait;
    Fish _fish;
    [ShowInInspector] Vector3 _direction;
    [ShowInInspector] float _distance;
    IDisposable _wonderingDisposable;
    IDisposable _baitObservable;
    private void Awake()
    {
        _fish = GetComponent<Fish>();
    }
    public void SetupBait(Bait bait)
    {
        _bait = bait;
        AddListener();
    }
    void AddListener()
    {
        _baitObservable?.Dispose();
        _baitObservable = _bait.ObserveEveryValueChanged(_ => _.HasFish).Where(hasfish => hasfish == true).Subscribe(_ => {
            if(_fish._bait == null)
                OnBaitHasGone.OnNext(default);
        }).AddTo(this);
    }

    public void Wandering()
    {
        
        _wonderingDisposable?.Dispose();
        _fish.StatsData.State = FishState.Wandering;

        var wonderingTimer = Random.Range(_fish.StatsData.TimeWanderingMin, _fish.StatsData.TimeWanderingMax);
         MoveAroundTarget(wonderingTimer);
        _wonderingDisposable = Observable.Timer(TimeSpan.FromSeconds(wonderingTimer)).Subscribe(_ => {
            Decide();
        }).AddTo(this);
    }
    void Decide()
    {
        var eatBait = Random.Range(0f, 1f);
        var StillInteresting = Random.Range(0, 1f);
        if (eatBait >= _fish.StatsData.RateToEatBait)
        {
            _fish.StatsData.State = FishState.Eating;
        }
        else
        {
            if (StillInteresting >= _fish.StatsData.RateToInteresting)
            {
                _fish.StatsData.State = FishState.Interesting;
            }else
            {

                OnBaitHasGone.OnNext(default);
            }
        }
    }
    private void Update()
    {
        if (_bait == null)
        {
            OnBaitHasGone.OnNext(default);
            return;
        }
        switch(_fish.StatsData.State)
        {
            case FishState.Interesting:
                MoveTargetWithInteresting();
                break;
            case FishState.Eating:
                MoveTargetWithEating();
                break;
        }
        UpdateRotation();
    }
    void UpdateRotation()
    {
        _direction = _bait.transform.position - transform.position;
        var angle = GameUtils.CalculateAngleFromDirection(_direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, angle, 0), 100 * Time.deltaTime);
    }
    void MoveTargetWithEating()
    {
        Move();
    }
    void MoveTargetWithInteresting()
    {
        if (_bait == null) return;
        _distance = Vector2.Distance(transform.position, _bait.transform.position);
        if (_distance < _fish.StatsData.DistanceToWandering)
        {
            Wandering();
            return;
        }
        Move();
    }
    void Move()
    {
        if (_bait == null) return;
        transform.position += (_direction.normalized) * _fish.StatsData.Speed * Time.deltaTime;
       
    }
    void MoveAroundTarget(float timer)
    {
        transform.DOKill();
        var randomSphere = Random.insideUnitSphere*3;
        randomSphere.y = 0;
        transform.DOMove(new Vector3(transform.position.x,transform.position.y,transform.position.z)+randomSphere, timer);
    }
    public void Dispose()
    {
        _wonderingDisposable?.Dispose();
        _bait = null;
        this.enabled = false;
    }
}
