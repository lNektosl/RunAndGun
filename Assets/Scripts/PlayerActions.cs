using System.Collections;
using UnityEngine;

public class PlayerActions : MonoBehaviour {


    [SerializeField] private BulletTrail bulletPrefab;

    private Vector2 smoothedMovment;
    private Vector2 smoothedVelocity;
    private Vector2 dashDir;
    private Vector2 moveDir;
    private PlayerInput input;
    private Vector2 lastInput;

    private Player player;
    private Rigidbody2D rigidbody2D;


    [SerializeField] private float rateOfFire;
    private bool readyToShoot = true;



    public void Initiate(Player player, Rigidbody2D rigidbody2D, PlayerInput input) {
        this.player = player;
        this.input = input;
        this.rigidbody2D = rigidbody2D;
    }

    public void Move(float speed) {
        moveDir = input.GetVector2Normalized();
        lastInput = moveDir;
        smoothedMovment = Vector2.SmoothDamp(smoothedMovment, moveDir, ref smoothedVelocity, 0.2f);
        rigidbody2D.velocity = smoothedMovment * speed;
    }

    public IEnumerator Dash(float dashSpeed, float dashDuration) {
        dashDir = lastInput;
        player.SetIsDashing(true);
        rigidbody2D.velocity = dashDir * dashSpeed;
        yield return new WaitForSeconds(dashDuration);
        player.SetIsDashing(false);
    }

    public void Shoot(Vector3 endOfAGun, Vector3 mousePosition) {
        if (readyToShoot) {
            readyToShoot = false;
            Vector3 aimDirection = (mousePosition - endOfAGun).normalized;
            float bulletSpeed = 40f;
            float weaponRange = 10f;
            RaycastHit2D hit = Physics2D.Raycast(endOfAGun, aimDirection, weaponRange);


            BulletTrail bullet = Instantiate(bulletPrefab, endOfAGun, transform.rotation);
            bullet.Initiate(endOfAGun, mousePosition, bulletSpeed);

            if (hit.collider) {
                bullet.SetTarget(hit.point);
                Debug.Log(hit.collider.gameObject.name);
            }
            else {
                Vector3 target = endOfAGun + weaponRange * aimDirection;
                target.z = 0;
                bullet.SetTarget(target);
            }
            Invoke("ResetShoot", rateOfFire);
        }
    }

    public void ResetShoot() {
        readyToShoot = true;
    }
}
