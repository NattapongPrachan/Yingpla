using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public Vector3 RodPosition;
    [SerializeField] Fish _fish;
    private void Start()
    {
    }
    private void Update()
    {
        var direction = transform.position - RodPosition;
        var angle = GameUtils.CalculateAngleFromDirection(direction);
        angle += Random.Range(-90, 90);
        transform.position += direction.normalized * _fish.FishStatsData.Speed* Time.deltaTime;

    }
}
