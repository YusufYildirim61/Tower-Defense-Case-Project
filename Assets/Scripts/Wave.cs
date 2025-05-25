using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave 
{
    public List<EnemySpawnData> enemySpawnDataList;
    public List<GameObject> enemiesInScene;
}

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public int minRandomAmount;
    public int maxRandomAmount;
}
