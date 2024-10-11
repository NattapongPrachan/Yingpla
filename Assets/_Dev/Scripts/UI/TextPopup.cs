using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextPopup : Popup
{
    TextMeshProUGUI _messageTxt;
    public TextPopup(string message)
    {
        _messageTxt.text = message;
    }
    public override void Create()
    {
        base.Create();

    }
}
