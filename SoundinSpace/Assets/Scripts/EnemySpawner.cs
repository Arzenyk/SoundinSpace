using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs; // Lista de prefabs de enemigos
    public int maxEnemies = 10;
    public float spawnRadius = 30f;
    public Transform player;
    public List<GameObject> enemies = new List<GameObject>();

    void Update()
    {
        // Mantener la cantidad de enemigos en el mapa
        if (enemies.Count < maxEnemies)
        {
            SpawnEnemy();
        }

        // Limpiar enemigos destruidos de la lista
        enemies.RemoveAll(e => e == null);
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0) return;

        Vector3 spawnPos = player.position + Random.onUnitSphere * spawnRadius;
        spawnPos.y = player.position.y; // Mantener en el mismo plano Y

        // Selecciona un prefab aleatorio
        int index = Random.Range(0, enemyPrefabs.Count);
        GameObject prefab = enemyPrefabs[index];

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        enemies.Add(enemy);
    }
}
