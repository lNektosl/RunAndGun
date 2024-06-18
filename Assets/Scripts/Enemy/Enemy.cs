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
    private float rotationSpeed = 360f;

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
    }

    private void FixedUpdate() {
        if (!isDead) {
            UpdateTargetDir();
            IsAware();
            HandleRotation();
            HandleMovment();
        }
    }

    private void HandleRotation() {
                if(targetDir!= Vector2.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, targetDir);
            Quaternion rotate = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            rigidbody.SetRotation(rotate);
        }
    }

    private void UpdateTargetDir() {
        toTargetVector = player.position - transform.position;
    }

    private void IsAware() {
        if (toTargetVector.magnitude <= awarensesRange) {
            isAwareOfThePlayer = true;
        }
        else {
            isAwareOfThePlayer = false;
        }
    }


    private void HandleMovment() {
       
        if (isAwareOfThePlayer) {
            targetDir = toTargetVector.normalized;
            rigidbody.velocity = targetDir * moveSpeed;
        }
        else {
            rigidbody.velocity = Vector2.zero;
        }
        isMoving = rigidbody.velocity != Vector2.zero;
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
        rigidbody.velocity = Vector2.zero;
        isMoving = false;
        isDead = true;
        collider.enabled = false;
        Destroy(gameObject, 3f);
    }
    public void OnCollisionStay2D(Collision2D collision) {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        if (damagable != null) {
            damagable.TakeDamage(damege);
        }
    }

}
