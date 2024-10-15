using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using Mono.Cecil;
using UnityEditor.Experimental.GraphView;
public class Popup : MonoBehaviour
{
    
    public bool IsMoving = false;
    [SerializeField] Vector2 _moveRectTransform;
    [SerializeField] float _duration = 1.0f;
    [SerializeField] Ease ease;
    [SerializeField] float _destroyDuration;
    private void Start()
    {
       // Moving();
    }
    public virtual void Moving()
    {
        if(IsMoving)
        {
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.DOMove(rectTransform.position + new Vector3(_moveRectTransform.x,_moveRectTransform.y,0), _duration).SetEase(ease).OnComplete(() => {

                Destroy(this.gameObject, _destroyDuration); 
            });
        }
    }
    public void SetRectPosition(Vector3 worldPosition)
    {
        var rectTransform = GetComponent<RectTransform>();
        var viewportPoint = Camera.main.WorldToScreenPoint(worldPosition);
        rectTransform.position = viewportPoint;
        Moving();
    }
}
