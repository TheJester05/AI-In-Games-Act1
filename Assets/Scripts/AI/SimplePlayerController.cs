using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class SimplePlayerController : MonoBehaviour
{
    public float speed = 5f;
    private CharacterController controller;

    void Start() => controller = GetComponent<CharacterController>();

    void Update()
    {
        Vector2 input = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) input.y += 1;
            if (Keyboard.current.sKey.isPressed) input.y -= 1;
            if (Keyboard.current.aKey.isPressed) input.x -= 1;
            if (Keyboard.current.dKey.isPressed) input.x += 1;
        }

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        controller.Move(move.normalized * speed * Time.deltaTime);

        if (!controller.isGrounded) controller.Move(Vector3.down * 9.81f * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic || hit.moveDirection.y < -0.3f) return;
        body.linearVelocity = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z) * speed * 0.5f;
    }
}
