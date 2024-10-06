using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Depug {

    internal static string Log(string message,Color? color)
    {
        color = color ?? Color.black;
        //Debug.LogFormat(string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color.Value), message));
        return string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color.Value), message);
    }
    
}
