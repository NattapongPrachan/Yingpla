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
        var randomPosition = AreaManager.RandomPosition();
        var distance = Vector3.Distance(randomPosition, transform.position);
        GetComponent<RotationDirection>().SetupTargetPosition(randomPosition);
        transform.DOMove(randomPosition, GameUtils.CalculateDistanceSpeedToTime(_speed, distance)).OnComplete(RandomMove);
    }
    // Update is called once per frame
    void Update()
    {
       // transform.position = AreaManager.GetOuterBounds();
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
