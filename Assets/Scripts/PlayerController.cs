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

    // Variables para mejorar el salto
    private float lastGroundedTime = 0f;
    private float lastJumpPressTime = -1f;
    private float coyoteTime = 0.15f;  // Tiempo después de salir del suelo donde puedes saltar
    private float jumpBufferTime = 0.2f;  // Tiempo que recuerda que presionaste salto
    private bool canDoubleJump = false;  // Para evitar saltos múltiples accidentales

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Si está muerto, no se mueve
        PlayerHealth health = GetComponent<PlayerHealth>();
        if (health != null && health.estaMuerto)
        {
            return;
        }

        // Guardar el tiempo que estuvimos en el suelo
        if (controller.isGrounded)
        {
            lastGroundedTime = Time.time;
            canDoubleJump = false;
        }

        // Resetear gravedad
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
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
        bool running = Keyboard.current.leftShiftKey.isPressed;
        animator.SetBool("isRunning", running);
        speed = running ? 7f : 4f;

        // Registrar cuando se presiona el salto
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            lastJumpPressTime = Time.time;
        }

        // Para evitar problemas con el salto, verificamos si el salto está dentro del tiempo de buffer y coyote time
        bool jumpBufferActive = (Time.time - lastJumpPressTime) < jumpBufferTime;
        bool coyoteTimeActive = (Time.time - lastGroundedTime) < coyoteTime;

        if (jumpBufferActive && coyoteTimeActive && !canDoubleJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = true;
            animator.SetBool("isJumping", true);
            canDoubleJump = true;  // Previene saltos múltiples
            lastJumpPressTime = -1f;  // Resetear para no saltar de nuevo
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