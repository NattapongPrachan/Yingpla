using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
[RequireComponent(typeof(BoxCollider2D))]
public class Fish : MonoBehaviour
{
    BoxCollider2D _boxCollider;
    [SerializeField]Transform _mouseTransform;
    [SerializeField]Transform _baitTransform;
    [SerializeField]FishStatsData _fishStatsData;
    
    private void Start()
    {
       
    }
    public void CatchBait(Transform baitTransofrm)
    {
        _baitTransform = baitTransofrm;
    }
    private void Update()
    {
        if(_baitTransform != null)
        {
            transform.position = _baitTransform.position;
        }
    }
    public void ReleaseBait()
    {
        GetComponent<RandomMovement>().Resume();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TagKeys.Bait))
        {
            if(collision.gameObject.TryGetComponent(out Bait bait) && !bait.HasFish)
            {
                bait.GetFish(this);
                GetComponent<RandomMovement>().Dispose();
                CatchBait(collision.transform);
            }
           
        }
       
    }
}
