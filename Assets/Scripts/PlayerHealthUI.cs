using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerHealthUI : MonoBehaviour
{
    public TextMeshProUGUI vidasText; // Arrastra tu texto del Canvas aquí

    private int maxVidas = 5;
    private int vidasActuales;

    void Start()
    {
        vidasActuales = maxVidas;
        ActualizarVidas();
    }

    public void PerderVida()
    {
        if (vidasActuales > 0)
        {
            vidasActuales--;
            ActualizarVidas();

            if (vidasActuales <= 0)
            {
                GameOver();
            }
        }
    }

    void ActualizarVidas()
    {
        if (vidasText != null)
        {
            vidasText.text = "Vidas restantes: " + vidasActuales;
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER - Cargando menú...");
        // Cargar escena del menú (ajusta el nombre de tu escena)
        SceneManager.LoadScene("MenuPrincipal");
    }
}