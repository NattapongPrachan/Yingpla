using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
[RequireComponent(typeof(BoxCollider2D))]
public class Fish : MonoBehaviour
{
    BoxCollider2D _boxCollider;
    [SerializeField] Transform _mouseTransform;
    Transform _baitTransform;
    private void Start()
    {
       
    }
    public void CatchBait(Transform baitTransofrm)
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("otherCollider " + collision);
        GetComponent<RandomMovement>().Dispose();
    }
}
