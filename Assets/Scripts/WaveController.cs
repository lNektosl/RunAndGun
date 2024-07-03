using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveController : MonoBehaviour {

    [SerializeField] GameObject spawnersHolder;
    private ISpawner[] spawners;
    private bool isWaveOn = false;
    private bool isGameOn = true;
    private int wave = 0;
    private int enemyCount = 0;
    private int curEnemyCount = 0;
    private int spawnedEnemyCount = 0;
    private float spawnTimer = 5;
    private int numOfWaves = 5;

    public void Start () {
        spawners = spawnersHolder.GetComponentsInChildren<ISpawner>();

    }

    public void FixedUpdate () {
        if (!isWaveOn && isGameOn) {
            StartWave();
        } else {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy () {
        if (spawnedEnemyCount < enemyCount) {
            if (spawnTimer <= 0) {
                int spawnerNum = Random.Range(0, spawners.Length);
                spawners[spawnerNum].Spawn();
                spawnTimer = Random.Range(1f, 5f);
                spawnedEnemyCount++;
                curEnemyCount++;
            }
            spawnTimer -= Time.deltaTime;
        }
    }

    public void DecreaseEnemyCount () {
        curEnemyCount--;
        if (curEnemyCount == 0) {
            isWaveOn = false;
        }
    }

    private void StartWave () {
        if (wave !=  numOfWaves) {

            isWaveOn = true;
            wave++;
            enemyCount = 4 * wave;
            curEnemyCount = 0;
            spawnedEnemyCount = 0;
        } else {
            Invoke("EndGame", 3f);
        }
    }

    private void EndGame () {
        SceneManager.LoadScene("Menu");
    }
}
