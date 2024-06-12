using UnityEngine;

public class MousePosition2D : MonoBehaviour {
    [SerializeField] private Camera camera;

    private Vector3 currentPosition;
    public void Start() {
        Cursor.visible = false;
    }
    public void Update() {
       MoveTarget();
    }

    public void MoveTarget() {
        currentPosition = GetMouseTargetPosition();
        transform.position = currentPosition;
    }
    public Vector3 GetMouseTargetPosition() {
        currentPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        currentPosition.z = 0;
        return currentPosition;
    }
}
