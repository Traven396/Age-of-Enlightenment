using AgeOfEnlightenment.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrimoireEnemySpawnerTab : MonoBehaviour, GrimoireTab
{
    public GrimoireManager Overhead { get; set; }


    public GameObject EnemyPrefab;
    public BaseAttackBehavior[] EnemyAttacks;

    [Space(10)]
    public Transform[] SpawnLocations;


    private List<GameObject> SpawnedEnemies;
    private void Awake()
    {
        SpawnedEnemies = new List<GameObject>();
    }

    public void SpawnEnemy()
    {
        int spawnNumber = Random.Range(0, SpawnLocations.Length - 1);

        var SpawnedEnemy = Instantiate(EnemyPrefab, SpawnLocations[spawnNumber].position, Quaternion.identity);

        GiveEnemyAttacks(SpawnedEnemy);

        SpawnedEnemies.Add(SpawnedEnemy);
    }


    public void ClearAllEnemies()
    {
        foreach (GameObject currentEnemy in SpawnedEnemies)
        {
            if (currentEnemy)
                Destroy(currentEnemy);
        }

        SpawnedEnemies = new List<GameObject>();
    }



    private void GiveEnemyAttacks(GameObject rootEnemy)
    {
        var enemyAI = rootEnemy.GetComponentInChildren<RagdollMageBrain>();
        for (int i = 0; i <= 3; i++)
        {
            var randomNum = Random.Range(0, EnemyAttacks.Length - 1);

            BaseAttackBehavior attackToAdd = EnemyAttacks[randomNum];

            if (!enemyAI.ListOfAttacks.Contains(attackToAdd))
                enemyAI.ListOfAttacks.Add(EnemyAttacks[randomNum]);
            else
                enemyAI.ListOfAttacks.Add(EnemyAttacks[(randomNum + 1) % EnemyAttacks.Length]);
        
        }
        enemyAI.CreateMageAttackBrain();
    }
}
