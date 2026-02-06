using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxVidas = 5;
    private int vidasActuales;
    private PlayerHealthUI healthUI;

    void Awake()
    {
        vidasActuales = maxVidas;
        healthUI = FindObjectOfType<PlayerHealthUI>();
    }

    public void RecibirDaño(float vidas)
    {
        vidasActuales -= (int)vidas;

        Debug.Log($"Player recibió {vidas} vida(s). Vidas restantes: {vidasActuales}");

        if (healthUI != null)
        {
            healthUI.PerderVida();
        }

        if (vidasActuales <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Debug.Log("Player muerto.");

        var controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        // Opcional: Esperar un momento antes de cargar el menú
        Invoke("CargarMenu", 2f); // Espera 2 segundos
    }

    void CargarMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipal");
    }
}