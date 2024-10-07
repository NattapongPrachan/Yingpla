using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bait : MonoBehaviour
{
    public bool HasFish { get; private set; }
    [SerializeField] Fish _fish;
    public void GetFish(Fish fish)
    {
        HasFish = true;
        _fish = fish;
        transform.DOKill();
    }
    public void Casting()
    {

    }
    public void Dispose()
    {
        _fish?.ReleaseBait();
        _fish = null;
        Destroy(gameObject);
    }
}
