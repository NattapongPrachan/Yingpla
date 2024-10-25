using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Canvas),typeof(CanvasGroup))]
public class BaseUI : SerializedMonoBehaviour
{
    Canvas _canvas;
    CanvasGroup _canvasGroup;
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    public virtual void Start()
    {
        UIManager.Register(this);
    }
    public void Focus() { _canvas.sortingOrder = 1; }
    public void UnFocus() { _canvas.sortingOrder = 0; }
    public void Dispose()
    {
        if(gameObject.TryGetComponent(out ReturnToPool returnToPool))
        {
            returnToPool.Release();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    

    
}
