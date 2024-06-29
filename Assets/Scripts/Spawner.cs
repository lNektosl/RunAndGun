using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, ISpawner {

    [SerializeField] private Enemy enemy;

    public void Spawn () {
        Vector3 posistion = transform.position;
        posistion.z = 0;
        Instantiate(enemy,posistion,Quaternion.identity);
    }

    // Start is called before the first frame update
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }
}
