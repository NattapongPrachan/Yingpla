using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
public class FleeMovement : MonoBehaviour
{
    public Vector3 BaitPosition;
    [SerializeField] Fish _fish;
    [SerializeField] float _randomInAngle;
    [ShowInInspector] Vector3 _direction;
    [SerializeField] float TimeSecound = 1;
    [ShowInInspector]Vector3 _randomDirection = Vector3.zero;
    IDisposable _timeRandomDirectionObservable;
    float _randomTime;
    private void Start()
    {
        RandomDirection();
    }
    void RandomDirection()
    {
        _randomTime = UnityEngine.Random.Range(0, TimeSecound);
        _timeRandomDirectionObservable = Observable.Timer(TimeSpan.FromSeconds(_randomTime)).Subscribe(_ => {
            _randomTime = UnityEngine.Random.Range(0, TimeSecound);
            _randomDirection = GetRandomDirection();
            RandomDirection();
        }).AddTo(this);
    }
    private void Update()
    {
        _direction = transform.position - BaitPosition;
        var angle = GameUtils.CalculateAngleFromDirection(_direction);
        transform.rotation = Quaternion.Euler(0, angle, 0);
        transform.position += (_direction.normalized+ _randomDirection.normalized) * (_fish.DiffSpeed)* Time.deltaTime;
        _fish.StatsData.MoveDirection = new Vector2(_direction.x,_direction.z);
    }

    Vector2 GetRandomDirection()
    {
        float angle = UnityEngine.Random.Range(-_randomInAngle, _randomInAngle);
        float radians = angle * Mathf.Rad2Deg;
        return new Vector3(Mathf.Cos(radians),0, Mathf.Sin(radians)); 
    }
    public void Dispose()
    {
        _timeRandomDirectionObservable?.Dispose();
        this.enabled = false;
    }
}
