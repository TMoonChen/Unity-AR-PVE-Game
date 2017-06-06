using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMoonObjectPool;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemies;
    public Transform[] spawnPostions;
    public Transform spawnParent;
    public float spawnWait;
    public float spawnMostWait;
    public float spawnLeasWait;
    public int startWait;
    public bool stop = false;
    public int maxCount = 5;
    public static int currentCount = 0;
    int randEnemy;
    int randPosition;

    private void Start()
    {
        currentCount = 0;

        for (int i = 0; i < enemies.Length; i++)
        {
            PoolManager.Instance.Create(enemies[i].name, enemies[i], spawnParent);
        }

        StartCoroutine(WaitSpawner());
    }

    private void Update()
    {
        spawnWait = Random.Range(spawnLeasWait, spawnMostWait);
    }

    IEnumerator WaitSpawner()
    {
        yield return new WaitForSeconds(startWait);
        while (!stop)
        {
            if (currentCount < maxCount)
            {
                randEnemy = Random.Range(0, enemies.Length);
                randPosition = Random.Range(0, spawnPostions.Length);

                PoolManager.Instance.GetPool(enemies[randEnemy].name).Allocate(spawnPostions[randPosition].position, Quaternion.identity);

                currentCount++;

                yield return new WaitForSeconds(spawnWait);
            }
            yield return 0;
        }
  
    }

    public void StartSpawn()
    {
        StartCoroutine(WaitSpawner());
    }

    private void OnDestroy()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            PoolManager.Instance.GetPool(enemies[i].name).Clear();
        }
        PoolManager.Instance.RemoveAll();
    }
}
