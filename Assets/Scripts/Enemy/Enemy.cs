using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour, IDamagable {

    private Transform player;

    private bool isAwareOfThePlayer = false;
    private bool isMoving = false;
    private bool isDead = false;

    private bool isActive = false;
    private float hp = 6;
    private Collider2D collider;
    private Rigidbody2D rigidbody;
    private WaveController waveController;

    private const float MOVE_SPEED = 2f;
    private float curMoveSpeed;
    private float rotationSpeed = 360f;
    private float awarensesRange = 5f;

    private Vector2 toTargetVector;
    private Vector2 targetDir;
    private Vector2 smoothedMovement;
    private Vector2 smoothedVelocity;

    private float damage = 2;
    float dirCouldown;


    private void Awake () {
        rigidbody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>().transform;
        collider = GetComponent<Collider2D>();
        waveController = FindObjectOfType<WaveController>();

    }
    private void Start () {
        rigidbody.freezeRotation = true;
        targetDir = new Vector2(0, 0);
        dirCouldown = Random.Range(1f, 5f);
        Physics2D.IgnoreLayerCollision(6, 8, true);

        curMoveSpeed = MOVE_SPEED;
    }

    private void FixedUpdate () {
        if (!isDead && isActive) {
            UpdateTargetDir();
            IsAware();
            HandleMovment();
        }
        if (!isActive) {
            MoveToCenter();
        }
        HandleRotation();
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

        rigidbody.velocity = targetDir * curMoveSpeed;

    }

    private void GetRandomDir () {
        float angleChange = Random.Range(-90f, 90f);
        Quaternion rotation = Quaternion.AngleAxis(angleChange, transform.forward);
        targetDir = rotation * targetDir;
    }


    private void HandlePlayerTargeting () {
        if (isAwareOfThePlayer) {
            targetDir = toTargetVector.normalized;
            smoothedMovement = Vector2.SmoothDamp(smoothedMovement, targetDir, ref smoothedVelocity, 0.5f);
            rigidbody.velocity = smoothedMovement * curMoveSpeed;
        }
    }

    public void TakeDamage (float damage) {
        hp -= damage;
        if (hp <= 0) {
            Die();
        }
        curMoveSpeed = 0.5f;
        Invoke("ResetMoveSpeed", 0.5f);
    }

    public void ResetMoveSpeed () {
        curMoveSpeed = MOVE_SPEED;
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
        waveController.DecreaseEnemyCount();
        collider.enabled = false;
        Destroy(gameObject, 3f);
    }
    public void OnCollisionStay2D (Collision2D collision) {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        string iDamagamleName = collision.gameObject.name.Replace("(Clone)", "").Trim();
        if (damagable != null && iDamagamleName != "Enemy") {
            damagable.TakeDamage(damage);
        }
    }

    public void OnCollisionEnter2D (Collision2D collision) {
        GameObject gameObject = collision.gameObject;
        int layer = gameObject.layer;

        if (LayerMask.LayerToName(layer) == "Boundary" && isActive) {
            targetDir = -targetDir;
            rigidbody.velocity = targetDir * curMoveSpeed;
        }
    }

    public void OnTriggerExit2D (Collider2D collision) {
        isActive = true;
        Physics2D.IgnoreLayerCollision(6, 8, false);
    }


    private void MoveToCenter () {
        Vector2 screenCenter = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2));
        targetDir = (screenCenter - (Vector2)transform.position).normalized;
        rigidbody.velocity = targetDir * curMoveSpeed;
    }
    
    private void DestroIfWanderOff () {
        Vector2 enemyPositionToCamera = Camera.main.WorldToViewportPoint(transform.position);

        if((enemyPositionToCamera.x<0 ||
            enemyPositionToCamera.x >1 ||
            enemyPositionToCamera.y<0 ||
            enemyPositionToCamera.y>1 )&&
            isActive) {
            Die();
        }
    }
}
