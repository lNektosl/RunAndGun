using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour{
    [SerializeField] GameObject spawnersHolder;
    private ISpawner [] spawners;

    private float spawnTimer = 5;

    public void Start () {
        spawners = spawnersHolder.GetComponentsInChildren<ISpawner> ();
    
    }

    public void FixedUpdate () {
        SpawnEnemy();
    }

    private void SpawnEnemy () {


        if (spawnTimer <= 0) {
            int spawnerNum = Random.Range(0, spawners.Length);
            spawners[spawnerNum].Spawn();
            spawnTimer = Random.Range(1f,5f);
        }

        spawnTimer -= Time.deltaTime;
    }
}
