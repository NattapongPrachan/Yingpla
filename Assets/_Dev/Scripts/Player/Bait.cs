using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bait : MonoBehaviour
{
    
    public bool HasFish { get; private set; }
    public Vector3 BaitStart;
    public Rod Rod;
    public Collider2D AuraCollider;
    [SerializeField] Fish _fish;
    public void GetFish(Fish fish)
    {
        HasFish = true;
        _fish = fish;
        transform.DOKill();
    }
    public void Shock(int damage)
    {
        
        _fish?.TakeDamage(Random.Range(0,damage));
    }
    public void OpenAuraCollider()
    {
        AuraCollider.enabled = true;
    }
    public void Dispose()
    {
        _fish?.ReleaseBait();
        _fish = null;
        Destroy(gameObject);
    }
}
