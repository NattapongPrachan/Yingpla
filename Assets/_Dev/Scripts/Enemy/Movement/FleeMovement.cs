using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using Unity.VisualScripting;
using UnityEngine.UIElements;
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

    [Header("Debug")]
    [SerializeField] float _randomAngle;
    [SerializeField] float _rotateSpeed;
    private void Start()
    {
        //RandomDirection();
    }
    void RandomDirection()
    {
        _timeRandomDirectionObservable?.Dispose();
        _randomTime = UnityEngine.Random.Range(0, TimeSecound);
        _timeRandomDirectionObservable = Observable.Timer(TimeSpan.FromSeconds(_randomTime)).Subscribe(_ => {
            _randomTime = UnityEngine.Random.Range(0, TimeSecound);
            _randomDirection = GetRandomDirection();
            RandomDirection();
            _fish.RotationDirection.UpdateRotaiton(RandomRotation());
        }).AddTo(this);
    }
    private void Update()
    {
        _direction = transform.position - BaitPosition;
        
        //var newRot = Quaternion.Euler(0, angle + _randomAngle, 0);
        //transform.rotation = Quaternion.Lerp(transform.rotation, newRot, _rotateSpeed * Time.deltaTime);
        transform.position += transform.forward * (_fish.DiffSpeed) * Time.deltaTime;
        _fish.StatsData.MoveDirection = new Vector2(_fish.transform.forward.x, _fish.transform.forward.z);
    }
    

    Vector2 GetRandomDirection()
    {
        _randomAngle = UnityEngine.Random.Range(-_randomInAngle, _randomInAngle);
        float radians = _randomAngle * Mathf.Rad2Deg;
        return new Vector3(Mathf.Cos(radians),0, Mathf.Sin(radians)); 
    }
    Quaternion RandomRotation()
    {
        var angle = GameUtils.CalculateAngleFromDirection(_direction);
        return Quaternion.Euler(0, angle + _randomAngle, 0);
    }
    private void OnEnable()
    {
        RandomDirection();
    }
    private void OnDisable()
    {
        
    }
    public void Dispose()
    {
        _timeRandomDirectionObservable?.Dispose();
        this.enabled = false;
    }
}
