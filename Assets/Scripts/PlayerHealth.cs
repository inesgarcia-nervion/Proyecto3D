using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxVidas = 5;
    private int vidasActuales;
    private PlayerHealthUI healthUI;

    public bool estaMuerto = false;

    void Awake()
    {
        vidasActuales = maxVidas;
        healthUI = FindObjectOfType<PlayerHealthUI>();

        if (healthUI != null)
            healthUI.ActualizarVidas(vidasActuales);
    }

    public void RecibirDaño(float daño)
    {
        if (estaMuerto) return;

        vidasActuales -= (int)daño;

        if (healthUI != null)
            healthUI.ActualizarVidas(vidasActuales);

        if (vidasActuales <= 0)
            Morir();
    }

    void Morir()
    {
        estaMuerto = true;
        GetComponent<PlayerController>().enabled = false;
        Invoke("CargarMenu", 2f);
    }

    void CargarMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuPrincipal");
    }
}
