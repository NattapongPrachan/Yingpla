using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoInstance<UIManager>
{
	[SerializeField]Dictionary<string , BaseUI> _uiCollection;
    public override void Init()
    {
        base.Init();
        _uiCollection = new Dictionary<string, BaseUI>();
    }

    public static void Register(BaseUI baseUI)
    {
        if(!Instance._uiCollection.ContainsKey(baseUI.name))
            Instance._uiCollection.Add(baseUI.name, baseUI);
    }
    public static void Unregister(BaseUI baseUI)
    {
        if(Instance._uiCollection.ContainsValue(baseUI))
        {
            Instance._uiCollection.Remove(baseUI.name);
        }
    }
    public static void Clear()
    {
        foreach (var ui in Instance._uiCollection.Values)
        {
            ui.Dispose();
        }
        Instance._uiCollection.Clear();
    }

    
}
