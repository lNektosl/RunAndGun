using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable {
    [SerializeField] private MousePosition2D mousePosition;
    [SerializeField] private PlayerInput input;
    [SerializeField] private Transform endOfAGun;
    [SerializeField] private BulletTrail bulletPrefab;

    private bool isShooting = false;
    private bool isDashing = false;
    private bool isMoving = false;
    private bool readyToShoot = true;
    private bool isDamagable = true;
    private bool isDead = false;

    private Rigidbody2D rigidbody2D;
    private Collider2D collider;

    private float speed = 7f;
    private int hp = 10;
    private float invulnerabilityDuration = 2f;
    private float dashSpeed = 17f;
    private float dashDuration = 0.3f;
    private int gunDamage = 2;
    private float rateOfFire = 0.5f;

    private Vector2 smoothedMovement;
    private Vector2 smoothedVelocity;
    private Vector2 dashDir;
    private Vector2 lastInput;

    public void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    public void Start() {
        input.OnShiftPressed += Dash_OnShiftPressed;
        input.OnShootStart += Shoot_OnShootStart;
        input.OnShootEnd += EndShoot_OnShootEnd;
        rigidbody2D.freezeRotation = true;
    }

    public void FixedUpdate() {
        if (!isDead) {
            HandleRotation();
            if (!isDashing) {
                HandleMovement();
            }
            if (isShooting) {
                HandleShooting();
            }
        }
    }

    private void HandleMovement() {
        Vector2 moveDir = input.GetVector2Normalized();
        if (moveDir != Vector2.zero) {
            lastInput = moveDir;
        }
        smoothedMovement = Vector2.SmoothDamp(smoothedMovement, moveDir, ref smoothedVelocity, 0.2f);
        rigidbody2D.velocity = smoothedMovement * speed;
        isMoving = moveDir != Vector2.zero;
    }

    private void HandleShooting() {
        if (readyToShoot) {
            readyToShoot = false;
            Vector3 aimDirection = (mousePosition.GetMouseTargetPosition() - endOfAGun.position).normalized;
            float bulletSpeed = 40f;
            float weaponRange = 10f;
            RaycastHit2D hit = Physics2D.Raycast(endOfAGun.position, aimDirection, weaponRange);

            BulletTrail bullet = Instantiate(bulletPrefab, endOfAGun.position, transform.rotation);
            bullet.Initiate(endOfAGun.position, mousePosition.GetMouseTargetPosition(), bulletSpeed);

            if (hit.collider) {
                bullet.SetTarget(hit.point);
                IDamagable damagableObject = hit.collider.GetComponent<IDamagable>();
                if (damagableObject != null) {
                    damagableObject.TakeDamage(gunDamage);
                }
            }
            else {
                Vector3 target = endOfAGun.position + weaponRange * aimDirection;
                target.z = 0;
                bullet.SetTarget(target);
            }
            Invoke("ResetShoot", rateOfFire);
        }
    }

    private void ResetShoot() {
        readyToShoot = true;
    }

    private void HandleRotation() {
        Vector3 aimDirection = GetAimDirection();
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private Vector3 GetAimDirection() {
        return (mousePosition.GetMouseTargetPosition() - transform.position);
    }

    private void Dash_OnShiftPressed(object sender, EventArgs e) {
        StartCoroutine(Dash());
    }

    private IEnumerator Dash() {
        dashDir = lastInput;
        isDashing = true;
        rigidbody2D.velocity = dashDir * dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    private void Shoot_OnShootStart(object sender, EventArgs e) {
        isShooting = true;
    }

    private void EndShoot_OnShootEnd(object sender, EventArgs e) {
        isShooting = false;
    }

    public bool IsMoving() {
        return isMoving;
    }

    public bool IsDead() {
        return isDead;
    }

    public void TakeDamage(int damage) {
        if (isDamagable) {
            isDamagable = false;
            Debug.Log("Ouch");
            hp -= damage;
            if (hp <= 0) {
                Die();
            }
            StartCoroutine("ResetISDamageble");
        }
    }
    public IEnumerator ResetISDamageble() {
        yield return new WaitForSeconds(invulnerabilityDuration);
        isDamagable = true;
    }
    private void Die() {
        isDead = true;
        collider.enabled = false;
        rigidbody2D.velocity = Vector2.zero;
    }
}
