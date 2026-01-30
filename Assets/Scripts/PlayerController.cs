using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.81f;
    private Vector2 moveInput;
    private Vector3 velocity;
    private float jumpHeight;
    public float range = 50f; // Alcance del arma
    public float damage = 10f;

    void Update()
    {
        // 1. Resetear gravedad SI toca el suelo (IMPORTANTE: Hacerlo al principio)

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -1f;
        }
        // 2. Lectura del Nuevo Input System (Teclado/WASD) y salto
        // Keyboard.current detecta el teclado activo

        if (Keyboard.current != null)
        {
            float x = 0;
            float z = 0;

            if (Keyboard.current.wKey.isPressed) z = 1;
            if (Keyboard.current.sKey.isPressed) z = -1;
            if (Keyboard.current.aKey.isPressed) x = -1;
            if (Keyboard.current.dKey.isPressed) x = 1;
            /* Aquí va el código de correr */
            moveInput = new Vector2(x, z);
            /* Aquí va el código del salto */
        }

        // 3. Cálculo de dirección
        // Calculamos la dirección horizontal (WASD)
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Aplicamos la gravedad al valor actual de velocity.y
        velocity.y += gravity * Time.deltaTime;

        // COMBINAMOS TODO: (Movimiento Horizontal * Velocidad) + Dirección Vertical
        Vector3 finalMovement = (move * speed) + (Vector3.up * velocity.y);
        controller.Move(finalMovement * Time.deltaTime);

        // 4. Saltar y correr
        // Para correr
        if (Keyboard.current.leftShiftKey.isPressed)
            speed = 20f;
        else
            speed = 12f;

        // Salto continuo
        if (Keyboard.current.spaceKey.isPressed && controller.isGrounded)
            velocity.y = 5f;

        // Un solo salto
        if (Keyboard.current.spaceKey.wasPressedThisFrame && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);    // jumpHeight = nº metros
        }

        // Agacharse
        transform.localScale = new Vector3(1f, (Keyboard.current.qKey.isPressed) ? 0.5f : 1f, 1f);


        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Disparar();
        }

        void Disparar()
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, range))
            {
                // ¿Hemos dado a un enemigo?
                EnemyHealth enemigo = hit.transform.GetComponent<EnemyHealth>();
                if (enemigo != null)
                {
                    enemigo.RecibirDaño(damage);
                }
            }
        }
    }
}
