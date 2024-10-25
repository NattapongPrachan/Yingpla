using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoInstance<UIManager>
{
	[SerializeField]Dictionary<string , BaseUI> _uiCollection;
	protected void Start()
	{
		_uiCollection = new Dictionary<string, BaseUI>();
        foreach (BaseUI canvas in _uiCollection.Values)
        {
        }
    }
    public static void Register(BaseUI baseUI)
    {
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
