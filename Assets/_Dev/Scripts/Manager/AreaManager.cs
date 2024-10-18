using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[DefaultExecutionOrder(10)]
public class AreaManager : MonoInstance<AreaManager>
{
    public Vector3 ScreenSize = new Vector3(1920,0,1080);
    public Vector2 SpawnOffsset = new Vector2(300,100);
    public Bounds AreaBounds;

    public override void Init()
    {
        base.Init();
        Random.InitState(Random.Range(0, 100));
        AreaBounds = new Bounds(Vector3.zero, new Vector3(ScreenSize.x, 0, ScreenSize.z));
    }
    public static Vector3 GetPosition(Vector3 worldPosition)
    {
        return new Vector3(worldPosition.x / GameOrthoGraphicSize.Instance.pixelsPerUnit,
                           worldPosition.y / GameOrthoGraphicSize.Instance.pixelsPerUnit,
                           worldPosition.z / GameOrthoGraphicSize.Instance.pixelsPerUnit);
    }
    public static Vector3 RandomPosition()
    {
        return GetPosition(new Vector3(Random.Range(Instance.AreaBounds.min.x, Instance.AreaBounds.max.x),0,
                           Random.Range(Instance.AreaBounds.min.z, Instance.AreaBounds.max.z)));
    }

    public static Vector2 GetOuterBounds()
    {
        int side = Random.Range(0, 4);  // 0 = ซ้าย, 1 = ขวา, 2 = บน, 3 = ล่าง
        Vector2 randomPosition = Vector2.zero;
        switch (side)
        {
            case 0: // ซ้าย
                randomPosition = new Vector3(Instance.AreaBounds.min.x - Instance.SpawnOffsset.x,0, Random.Range(Instance.AreaBounds.min.z, Instance.AreaBounds.max.z));
                break;
            case 1: // ขวา
                randomPosition = new Vector3(Instance.AreaBounds.max.x + Instance.SpawnOffsset.x,0, Random.Range(Instance.AreaBounds.min.z, Instance.AreaBounds.max.z));
                break;
            case 2: // บน
                randomPosition = new Vector3(Random.Range(Instance.AreaBounds.min.x, Instance.AreaBounds.max.x),0, Instance.AreaBounds.max.z + Instance.SpawnOffsset.y);
                break;
            case 3: // ล่าง
                randomPosition = new Vector3(Random.Range(Instance.AreaBounds.min.x, Instance.AreaBounds.max.x),0, Instance.AreaBounds.min.z - Instance.SpawnOffsset.y);
                break;
        }
        return GetPosition(randomPosition);
    }
    public static Vector2 GetInerBounds()
    {
        return new Vector3(Random.Range(Instance.AreaBounds.min.x, Instance.AreaBounds.max.x),
                                                0,
                                                Random.Range(Instance.AreaBounds.min.y, Instance.AreaBounds.max.y));
    }
}
