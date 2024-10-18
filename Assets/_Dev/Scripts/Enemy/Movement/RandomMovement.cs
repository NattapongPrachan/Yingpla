using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.Entities;
using UnityEngine;
[DefaultExecutionOrder(88)]
public class RandomMovement : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _reachToDistance = 1;
    [SerializeField] Vector3 _randomPosition;
    [SerializeField] float _distance;
    void Awake()
    {
        transform.position = AreaManager.GetOuterBounds();
        //RandomMove();
        //AddListener();
    }
    void AddListener()
    {
        PlayerTouchScreen.OnSelectWorldPoint.Subscribe(worldPoint =>
        {
            transform.position = worldPoint;
        }).AddTo(this);
    }
    void RandomMove()
    {
        _randomPosition = AreaManager.RandomPosition();//randomposition พัง
        var distance = Vector3.Distance(_randomPosition, transform.position);
        GetComponent<RotationDirection>().SetupTargetPosition(_randomPosition);
       // transform.DOMove(randomPosition, GameUtils.CalculateDistanceSpeedToTime(_speed, distance)).OnComplete(RandomMove);
    }
    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;
        _distance = Vector3.Distance(transform.position, _randomPosition);
        if (_distance < _reachToDistance)
        {
            RandomMove();
        }
    }
    private void OnEnable()
    {
        RandomMove();
    }
    public void Dispose()
    {
        transform.DOKill();
        this.enabled = false;
    }
}
