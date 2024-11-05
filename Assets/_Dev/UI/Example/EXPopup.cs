using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class EXPopup : BaseUI
{
    [SerializeField] private Button _closeBtn;
    public override void Start()
    {
        base.Start();
        _closeBtn.OnClickAsObservable().Subscribe(_ =>
        {
            Dispose();

        }).AddTo(this);
    }
}
