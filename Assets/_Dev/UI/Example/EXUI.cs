using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EXUI : BaseUI
{
    // Start is called before the first frame update

    void Start()
    {
        StartCoroutine(LoadCanvas());
    }

    IEnumerator LoadCanvas()
    {
        var canvas1 = Resources.LoadAsync<GameObject>("ExCanvas1");
        yield return canvas1;
        Instantiate(canvas1.asset);
    }
    private void OnLoadCompleted(AsyncOperation obj)
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
