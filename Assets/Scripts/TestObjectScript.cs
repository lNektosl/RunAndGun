using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObjectScript : MonoBehaviour, IDamagable {
    [SerializeField] int HP = 5;
    private Collider2D colider;

    private void Start() { 
    colider = GetComponent<Collider2D>();
    }
    public void TakeDamage(int damage) {
        HP -= damage;
        Debug.Log(HP);
        if (HP <= 0) {
            Debug.Log("dead");
            Die();
        }
    }
    private void Die() {
        colider.enabled = false;
        Destroy(this.gameObject,4);
    }
}
