using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour, IDamagable {

    private Transform player;

    private bool isActive = false;
    private int hp = 6;
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
    float dirCouldown;

    private bool isDead = false;
    private void Awake () {
        rigidbody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>().transform;
        collider = GetComponent<Collider2D>();


    }
    private void Start () {
        rigidbody.freezeRotation = true;
        targetDir = new Vector2(0, 0);
        dirCouldown = Random.Range(1f, 5f);
        Physics2D.IgnoreLayerCollision(6, 8, true);

    }

    private void FixedUpdate () {
        if (!isDead || isActive) {
            UpdateTargetDir();
            IsAware();
            HandleRotation();
            HandleMovment();
        }
        if (!isActive) {
            MoveToCenter();
        }
    }

    private void HandleRotation () {
        if (targetDir != Vector2.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, targetDir);
            Quaternion rotate = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            rigidbody.SetRotation(rotate);
        }
    }

    private void UpdateTargetDir () {
        toTargetVector = player.position - transform.position;
    }

    private void IsAware () {
        if (toTargetVector.magnitude <= awarensesRange) {
            isAwareOfThePlayer = true;
        } else {
            isAwareOfThePlayer = false;
        }
    }


    private void HandleMovment () {
        HandleRandomDirection();
        HandlePlayerTargeting();
        isMoving = rigidbody.velocity != Vector2.zero;
    }

    private void HandleRandomDirection () {

        dirCouldown -= Time.deltaTime;

        if (dirCouldown <= 0 && isActive) {
            GetRandomDir();
            dirCouldown = Random.Range(1f, 5f);
        }

        rigidbody.velocity = targetDir * moveSpeed;

    }

    private void GetRandomDir () {
        float angleChange = Random.Range(-90f, 90f);
        Quaternion rotation = Quaternion.AngleAxis(angleChange, transform.forward);
        targetDir = rotation * targetDir;
    }


    private void HandlePlayerTargeting () {
        if (isAwareOfThePlayer) {
            targetDir = toTargetVector.normalized;
            rigidbody.velocity = targetDir * moveSpeed;
        }
    }

    public void TakeDamage (int damage) {
        hp -= damage;
        if (hp <= 0) {
            Die();
        }
    }

    public bool IsMoving () {
        return isMoving;
    }

    public bool IsDead () {
        return isDead;
    }

    private void Die () {
        rigidbody.velocity = Vector2.zero;
        isMoving = false;
        isDead = true;
        collider.enabled = false;
        Destroy(gameObject, 3f);
    }
    public void OnCollisionStay2D (Collision2D collision) {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        if (damagable != null) {
            damagable.TakeDamage(damege);
        }
    }

    public void OnCollisionEnter2D (Collision2D collision) {
        GameObject gameObject = collision.gameObject;
        int layer = gameObject.layer;

        if (LayerMask.LayerToName(layer) == "Boundary" && isActive) {
            targetDir = -targetDir;
            rigidbody.velocity = targetDir * moveSpeed;
        }
    }

    public void OnTriggerExit2D (Collider2D collision) {
        isActive = true;
        Physics2D.IgnoreLayerCollision(6, 8, false);
    }


    private void MoveToCenter () {
        Vector2 screenCenter = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
        targetDir = (screenCenter - (Vector2)transform.position).normalized;
        rigidbody.velocity = targetDir * moveSpeed;
    }
}
