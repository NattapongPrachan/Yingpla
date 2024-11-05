using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class TextPopup : Popup
{
    [SerializeField]TextMeshProUGUI _messageTxt;
    

    public void SetText(string text)
    {
        _messageTxt.text = text; 
    }
    
}
