using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bait : MonoBehaviour
{
    
    public bool HasFish { get; private set; }
    public Vector3 BaitStart;
    public Rod Rod;
    public Collider AuraCollider;
    public Fish Fish { get; private set; }
    public void GetFish(Fish fish)
    {
        HasFish = true;
        Fish = fish;
        transform.DOKill();
    }
    public void Shock(int damage)
    {
        
        Fish?.TakeDamage(Random.Range(0,damage));
    }
    public void OpenAuraCollider()
    {
        AuraCollider.enabled = true;
    }
    public void Dispose()
    {
        Fish?.ReleaseBait();
        Fish = null;
        Destroy(gameObject);
    }
}
