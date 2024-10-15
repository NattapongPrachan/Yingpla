using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[DefaultExecutionOrder(10)]
public class AreaManager : MonoInstance<AreaManager>
{
    public Vector3 ScreenSize = new Vector3(1920,1080,0);
    public Vector2 SpawnOffsset = new Vector2(300,100);
    public Bounds AreaBounds;

    public override void Init()
    {
        base.Init();
        Random.InitState(Random.Range(0, 100));
        AreaBounds = new Bounds(Vector3.zero, new Vector3(ScreenSize.x, ScreenSize.y, 0));
    }
    public static Vector2 GetPosition(Vector2 worldPosition)
    {
        return new Vector2(worldPosition.x / GameOrthoGraphicSize.Instance.pixelsPerUnit,
                           worldPosition.y / GameOrthoGraphicSize.Instance.pixelsPerUnit);
    }
    public static Vector2 RandomPosition()
    {
        return GetPosition(new Vector2(Random.Range(Instance.AreaBounds.min.x, Instance.AreaBounds.max.x),
                           Random.Range(Instance.AreaBounds.min.y, Instance.AreaBounds.max.y)));
    }

    public static Vector2 GetOuterBounds()
    {
        int side = Random.Range(0, 4);  // 0 = ซ้าย, 1 = ขวา, 2 = บน, 3 = ล่าง
        Vector2 randomPosition = Vector2.zero;
        switch (side)
        {
            case 0: // ซ้าย
                randomPosition = new Vector2(Instance.AreaBounds.min.x - Instance.SpawnOffsset.x, Random.Range(Instance.AreaBounds.min.y, Instance.AreaBounds.max.y));
                break;
            case 1: // ขวา
                randomPosition = new Vector2(Instance.AreaBounds.max.x + Instance.SpawnOffsset.x, Random.Range(Instance.AreaBounds.min.y, Instance.AreaBounds.max.y));
                break;
            case 2: // บน
                randomPosition = new Vector2(Random.Range(Instance.AreaBounds.min.x, Instance.AreaBounds.max.x), Instance.AreaBounds.max.y + Instance.SpawnOffsset.y);
                break;
            case 3: // ล่าง
                randomPosition = new Vector2(Random.Range(Instance.AreaBounds.min.x, Instance.AreaBounds.max.x), Instance.AreaBounds.min.y - Instance.SpawnOffsset.y);
                break;
        }
        return GetPosition(randomPosition);
    }
}
