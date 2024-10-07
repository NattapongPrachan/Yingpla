using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
[DefaultExecutionOrder(88)]
public class RandomMovement : MonoBehaviour
{
    [SerializeField] float _speed;
    void Start()
    {
        transform.position = AreaManager.GetOuterBounds();
        RandomMove();
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
        var distance = Vector2.Distance(randomPosition, transform.position);
        transform.DOMove(AreaManager.RandomPosition(), GameUtils.CalculateDistanceSpeedToTime(_speed, distance)).OnComplete(RandomMove);
    }
    // Update is called once per frame
    void Update()
    {
       // transform.position = AreaManager.GetOuterBounds();
    }
}
