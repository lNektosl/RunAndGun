using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable {

    [SerializeField] private MousePosition2D mousePosition;
    [SerializeField] private PlayerInput input;
    [SerializeField] private Transform endOfAGun;
    [SerializeField] private BulletTrail bulletPrefab;
    [SerializeField] private Color flashColor;
    [SerializeField] private float numberOfFlashes;

    private bool isShooting = false;
    private bool isDashing = false;
    private bool isMoving = false;
    private bool readyToShoot = true;
    private bool isDamagable = true;
    private bool isDead = false;

    private Rigidbody2D rigidbody2D;
    private Collider2D collider;
    private SpriteRenderer spriteRenderer;

    private float speed = 7f;
    private const float HP = 10f;
    private float curHP = 10f;
    private float invulnerabilityDuration = 2f;
    private float dashSpeed = 17f;
    private float dashDuration = 0.3f;
    private float gunDamage = 2f;
    private float rateOfFire = 0.5f;
    private float flashDuration = 2f;

    private Vector2 smoothedMovement;
    private Vector2 smoothedVelocity;
    private Vector2 dashDir;
    private Vector2 lastInput;

    public delegate void PlayerDieEventHadler ();
    public event PlayerDieEventHadler OnPlayerDie;

    public void Awake () {
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Start () {
        input.OnShiftPressed += Dash_OnShiftPressed;
        input.OnShootStart += Shoot_OnShootStart;
        input.OnShootEnd += EndShoot_OnShootEnd;
        rigidbody2D.freezeRotation = true;
    }

    public void FixedUpdate () {
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

    private void HandleMovement () {
        Vector2 moveDir = input.GetVector2Normalized();
        if (moveDir != Vector2.zero) {
            lastInput = moveDir;
        }
        smoothedMovement = Vector2.SmoothDamp(smoothedMovement, moveDir, ref smoothedVelocity, 0.2f);
        rigidbody2D.velocity = smoothedMovement * speed;
        isMoving = moveDir != Vector2.zero;
    }

    private void HandleShooting () {
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
            } else {
                Vector3 target = endOfAGun.position + weaponRange * aimDirection;
                target.z = 0;
                bullet.SetTarget(target);
            }
            Invoke("ResetShoot", rateOfFire);
        }
    }

    private void ResetShoot () {
        readyToShoot = true;
    }

    private void HandleRotation () {
        Vector3 aimDirection = GetAimDirection();
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private Vector3 GetAimDirection () {
        return (mousePosition.GetMouseTargetPosition() - transform.position);
    }

    private void Dash_OnShiftPressed (object sender, EventArgs e) {
        StartCoroutine(Dash());
    }

    private IEnumerator Dash () {
        dashDir = lastInput;
        isDashing = true;
        rigidbody2D.velocity = dashDir * dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    private void Shoot_OnShootStart (object sender, EventArgs e) {
        isShooting = true;
    }

    private void EndShoot_OnShootEnd (object sender, EventArgs e) {
        isShooting = false;
    }

    public bool IsMoving () {
        return isMoving;
    }

    public bool IsDead () {
        return isDead;
    }

    public void TakeDamage (float damage) {
        if (isDamagable) {
            isDamagable = false;
            Debug.Log("Ouch");
            curHP -= damage;
            if (curHP <= 0) {
                Die();
                return;
            }
            StartCoroutine("ResetISDamageble");
            StartCoroutine(nameof(Flash));
        }
    }

    private IEnumerator Flash () {
        Color startColor = spriteRenderer.color;
        float elapsedFlashTime = 0;
        float elapsedFlashPrecentage = 0;

        while (elapsedFlashTime < flashDuration) {
            elapsedFlashTime += Time.deltaTime;
            elapsedFlashPrecentage = elapsedFlashTime/flashDuration;

            if (elapsedFlashPrecentage > 1) {
                elapsedFlashPrecentage = 1;
            }

            float pingPongPrecentage = Mathf.PingPong(elapsedFlashPrecentage * 2 * numberOfFlashes, 1);
            spriteRenderer.color = Color.Lerp(startColor, flashColor, pingPongPrecentage);

            yield return null;
               
        }


    }

    public float GetHPAsPercentage () {
        if (curHP <= 0) {
            return 0;
        }

        return curHP / HP;

    }
    public IEnumerator ResetISDamageble () {
        yield return new WaitForSeconds(invulnerabilityDuration);
        isDamagable = true;
    }
    private void Die () {
        isDead = true;
        collider.enabled = false;
        rigidbody2D.velocity = Vector2.zero;

        OnPlayerDie?.Invoke();
    }
}
