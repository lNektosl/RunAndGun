using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour, IDamagable {

    private Transform player;

    private int hp = 5;
    private Collider2D collider;

    private Rigidbody2D rigidbody;
    private bool isMoving = false;
    private float moveSpeed = 3f;

    private bool isAwareOfThePlayer = false;
    private float awarensesRange = 5f;

    private Vector2 toTargetVector;
    private Vector2 targetDir;
    private int damege = 2;

    private bool isDead = false;
    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>().transform;
        collider = GetComponent<Collider2D>();

        rigidbody.freezeRotation = true;
    }

    private void FixedUpdate() {
        if (!isDead) {
            UpdateTargetDir();
            IsAware();
            if (isAwareOfThePlayer) {
                HandleRotation();
                HandleMovment();
            }
        }
    }

    private void HandleRotation() {
        targetDir = toTargetVector.normalized;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void UpdateTargetDir() {
        toTargetVector = player.position - transform.position;
    }

    private void IsAware() {
        if (toTargetVector.magnitude <= awarensesRange) { 
        isAwareOfThePlayer = true;
        }
        else {
            StopMovment();
            isAwareOfThePlayer = false;
        }
    }


    private void HandleMovment() {
        isMoving = true;
        rigidbody.velocity = targetDir * moveSpeed;
    }

    private void StopMovment() {
        isMoving = false;
        rigidbody.velocity = Vector2.zero;
    }

    public void TakeDamage(int damage) {
        hp -= damage;
        if (hp <= 0) {
            Die();
        }
    }

    public bool IsMoving() {
        return isMoving;
    }

    private void Die() {
        StopMovment();
        isMoving = false;
        isDead = true;
        collider.enabled = false;   
        Destroy(gameObject, 3f);
    }
    public void OnCollisionStay2D(Collision2D collision) {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        if (damagable!=null){
            damagable.TakeDamage(damege);
        }
    }

}
