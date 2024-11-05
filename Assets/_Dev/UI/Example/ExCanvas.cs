using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ExCanvas : MonoBehaviour
{
    [SerializeField] private Button _callPopupBtn;
    void Start()
    {
        _callPopupBtn.OnClickAsObservable().Subscribe(_ =>
        {
            StartCoroutine(LoadPopup());
        }).AddTo(this);
    }
    IEnumerator LoadPopup()
    {
        var canvas1 = Resources.LoadAsync<GameObject>("Expopup");
        yield return canvas1;
        Instantiate(canvas1.asset);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
