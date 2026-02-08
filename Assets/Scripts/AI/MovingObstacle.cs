using UnityEngine;
using UnityEngine.InputSystem;

public class MovingObstacle : MonoBehaviour
{
    public Vector3 openPos, closedPos;
    public float speed = 2f;
    private bool isOpen;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.oKey.wasPressedThisFrame) isOpen = !isOpen;
        transform.position = Vector3.MoveTowards(transform.position, isOpen ? openPos : closedPos, speed * Time.deltaTime);
    }
}
