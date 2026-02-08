using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    private Vector2 moveInput;
    private Vector3 velocity;

    private Animator animator;
    private bool isJumping = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Si está muerto, no se mueve
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null && health.estaMuerto)
            return;

        // Resetear gravedad
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -1f;

            if (isJumping)
            {
                isJumping = false;
                animator.SetBool("isJumping", false);
            }
        }

        // Movimiento WASD
        if (Keyboard.current != null)
        {
            float x = 0;
            float z = 0;

            if (Keyboard.current.wKey.isPressed) z = 1;
            if (Keyboard.current.sKey.isPressed) z = -1;
            if (Keyboard.current.aKey.isPressed) x = -1;
            if (Keyboard.current.dKey.isPressed) x = 1;

            moveInput = new Vector2(x, z);
        }

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Animación de caminar/correr
        float currentSpeed = new Vector2(moveInput.x, moveInput.y).magnitude;
        animator.SetFloat("speed", currentSpeed);

        // Correr
        speed = Keyboard.current.leftShiftKey.isPressed ? 20f : 12f;

        // SALTO (el bueno)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            animator.SetBool("isJumping", true);
        }

        // Gravedad
        velocity.y += gravity * Time.deltaTime;

        // Movimiento final
        Vector3 finalMovement = (move * speed) + (Vector3.up * velocity.y);
        controller.Move(finalMovement * Time.deltaTime);

        // Agacharse
        transform.localScale = new Vector3(1f, Keyboard.current.qKey.isPressed ? 0.5f : 1f, 1f);
    }
}
