using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnMenu : MonoBehaviour
{
    public List<GameObject> enemyList;
    public List<Transform> spawnPoints;

    private int lastSpawnPoint;
    public void SpawnEnemy()
    {
        int spawnPoint = Random.Range(0, spawnPoints.Count);
        while(lastSpawnPoint == spawnPoint)
        {
            spawnPoint = Random.Range(0, spawnPoints.Count);
        }
        lastSpawnPoint = spawnPoint;

        Instantiate(enemyList[Random.Range(0, enemyList.Count)], spawnPoints[spawnPoint]);
    }
}
