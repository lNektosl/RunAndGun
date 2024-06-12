using System;
using UnityEngine;
public class Player : MonoBehaviour {


    [SerializeField] private MousePosition2D mousePosition;
    [SerializeField] private PlayerActions playerActions;
    [SerializeField] private PlayerInput input;
    [SerializeField] private Transform endOfAGun;

    private bool isShoting = false;
    private bool isDashing = false;
    
    private Rigidbody2D rigidbody2D;

    private float speed = 7f;
    private float dashSpeed = 17f;
    private float dashDuration = 0.3f;

    public void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();

    }

    public void Start() {
        playerActions.Initiate(this, rigidbody2D, input);
        input.OnShiftPressed += Dash_OnShiftPressed;
        input.OnShootStart += Shoot_OnShootStart;
        input.OnShootEnd += EndShoot_OnShootEnd;
    }

    public void FixedUpdate() {
        HandleRotation();
        if (!isDashing) {
            HandleMovement();
        }
        if (isShoting) {
            HandleShooting();
        }


    }

    private void HandleMovement() {
        playerActions.Move(speed);
    }
    private void HandleShooting() {
            playerActions.Shoot(endOfAGun.position, mousePosition.GetMouseTargetPosition());
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
        StartCoroutine(playerActions.Dash(dashSpeed, dashDuration));
    }

    private void Shoot_OnShootStart(object sender, EventArgs e) {
        isShoting = true;
    }
    private void EndShoot_OnShootEnd(object sender, EventArgs e) { 
        isShoting = false; 
    }

    public void SetIsDashing(bool isDashing) {
        this.isDashing = isDashing;
    }
}