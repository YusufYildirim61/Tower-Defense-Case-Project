using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] List<Wave> waveEnemiesList;
    [SerializeField] float[] waitTimeList;
    [SerializeField] Transform spawnArea;
    [SerializeField] Transform enemyPool;
    
    GameManager gameManager;
    Vector3 center;
    Vector3 size;
    
    int waitTimeListIndex = 0;
    int openWavesIndex = 0;
    bool gameOver = false;

    void OnEnable() 
    {
        GameEvents.OnEnemyKill += CheckLevelFinish;
    }
    void OnDisable() 
    {
        GameEvents.OnEnemyKill -= CheckLevelFinish;
    }
    private void Awake() 
    {
        center = spawnArea.position;
        size = spawnArea.localScale;

        for (int i = 0; i < waveEnemiesList.Count; i++)
        {
            Spawn();
        }
    }
    void Start()
    {
        GameEvents.OnNextWave?.Invoke(0,waveEnemiesList.Count);

        gameManager = GameManager.Instance;

        gameManager.TotalEnemies = 0;
        foreach (var wave in waveEnemiesList)
        {
            gameManager.TotalEnemies += wave.enemiesInScene.Count;
        }
    }
    
    void EnableObjectInPool() 
    {
        if(!gameOver)
        {
            foreach (GameObject enemies in waveEnemiesList[openWavesIndex].enemiesInScene)
            {
                enemies.SetActive(true);
            }
            return;
        }
    }
    public void StartWaves()
    {
        StartCoroutine(SpawnEnemy());
    }
    public void CallNextWave() // Next Wave Button 
    {
        if(openWavesIndex>=waitTimeList.Length)
        {
            return;
        }

        StopAllCoroutines();
        EnableObjectInPool();

        openWavesIndex++;
        GameEvents.OnNextWave?.Invoke(openWavesIndex,waveEnemiesList.Count);
        
        if(openWavesIndex<waitTimeList.Length)
        {
            StartCoroutine(SpawnEnemy());
        }
    }
    void CheckLevelFinish()
    {
        gameManager.KilledEnemies++;
        if(gameManager.KilledEnemies == gameManager.TotalEnemies && !gameManager.GameOver)
        {
            GameEvents.OnLevelComplete?.Invoke();
        }
    }
    IEnumerator SpawnEnemy()
    {
        while(openWavesIndex<waitTimeList.Length)
        {
            yield return new WaitForSeconds(waitTimeList[openWavesIndex]);
            EnableObjectInPool();
            openWavesIndex++;
            GameEvents.OnNextWave?.Invoke(openWavesIndex,waveEnemiesList.Count);
        }
    }
    private void Spawn()
    {
        Wave currentWave = waveEnemiesList[waitTimeListIndex];

        foreach (var enemyData in currentWave.enemySpawnDataList)
        {
            int enemyCount = Random.Range(enemyData.minRandomAmount, enemyData.maxRandomAmount + 1);

            for (int i = 0; i < enemyCount; i++)
            {
                Vector2 spawnPos = GetRandomPosition(center, size);

                GameObject enemyInstance = Instantiate(enemyData.enemyPrefab, spawnPos, enemyData.enemyPrefab.transform.rotation,enemyPool);
                enemyInstance.SetActive(false);
                currentWave.enemiesInScene.Add(enemyInstance);
            }
        }
        waitTimeListIndex++;
    }
    Vector2 GetRandomPosition(Vector3 center, Vector3 size)
    {
        float x = Random.Range(-0.5f, 0.5f) * size.x;
        float y = Random.Range(-0.5f, 0.5f) * size.y;
        return new Vector2(center.x + x, center.y + y);
    }
    
}
