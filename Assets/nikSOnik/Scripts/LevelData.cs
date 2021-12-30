using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "nikSOnik/Data/New Level")]
public class LevelData : ScriptableObject
{
    [SerializeField] Level data;
    public Level Data => data;

    public void Initialize()
    {
        for (int i = 0; i < data.bricks.Length; i++)
        {
            BrickData brick = MainManager.Instance.GetBrickData(data.bricks[i].id);
            data.bricks[i].lives = brick.HitsToDestroy;
        }
    }
}