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
    public FishStatsData FishStatsData;
    public float rand;
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
            rand = Random.Range(-2, 2);
         
            if(FishStatsData.State == FishState.Flee)
            {
                if(rand > 0)
                {
                    //Debug.Log("A");
                    transform.position = _baitTransform.position;
                }
                else
                {
                    //Debug.Log("B");
                    _baitTransform.position = transform.position;
                }
                
            }
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
                GetComponent<FishMovement>().enabled = true;
                GetComponent<FishMovement>().RodPosition = bait.BaitStart;
                GetComponent<RandomMovement>().Dispose();
                FishStatsData.State = FishState.Flee;
                CatchBait(collision.transform);
            }
           
        }
       
    }
}
