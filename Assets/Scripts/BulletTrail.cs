using UnityEngine;

public class BulletTrail : MonoBehaviour{

    private float progress;
    private Vector3 endOfAGun;
    private Vector3 target;
    private float speed;


    public void Fly() {
        progress += speed * Time.deltaTime;
        transform.position = Vector3.Lerp(endOfAGun, target, progress);
        if (progress >= 1f) {
            Destroy(this);
        }
    }

    public void Update() { 
        Fly();
    }

    public void Initiate(Vector3 endOfAGun,Vector3 target, float speed) {
        this.endOfAGun = endOfAGun;
        this.target = target;
        this.speed = speed;
    }

    public void SetTarget(Vector3 target) { 
    this.target=target;
        Debug.Log(this.target);
    }
}

