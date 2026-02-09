using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BaulScript : MonoBehaviour
{
    public int monedasGuardadas = 0;
    public int monedasParaGanar = 15;
    public TextMeshProUGUI baulText;

    [Header("Victoria")]
    public GameObject panelVictoria;
    public float tiempoAntesDeSalir = 3f;

    private bool juegoTerminado = false;

    void Start()
    {
        UpdateHUD();

        if (panelVictoria != null)
            panelVictoria.SetActive(false);
    }

    // Este método es llamado desde Inter.cs cuando presionas E
    public void Interactuar()
    {
        if (juegoTerminado)
        {
            return;
        }

        GuardarMonedas();
    }

    void GuardarMonedas()
    {
        if (CoinManager.Instance == null)
        {
            return;
        }

        // Verificar si el jugador tiene monedas
        if (CoinManager.Instance.coinsCollected <= 0)
        {
            return;
        }

        // Transferir una moneda del jugador al baúl
        monedasGuardadas += 1;
        CoinManager.Instance.coinsCollected -= 1;

        // Actualizar ambos HUD
        CoinManager.Instance.UpdateHUD();
        UpdateHUD();


        // Comprobar victoria
        if (monedasGuardadas >= monedasParaGanar)
        {
            Victoria();
        }
    }

    void UpdateHUD()
    {
        if (baulText != null)
            baulText.text = "Monedas en el baúl: " + monedasGuardadas + "/" + monedasParaGanar;
    }

    void Victoria()
    {
        juegoTerminado = true;

        // Desactivar controles del jugador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerController pc = playerObj.GetComponent<PlayerController>();
            if (pc != null) pc.enabled = false;
        }

        // Mostrar panel de victoria
        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }

        // Detener el tiempo
        Time.timeScale = 0f;

        // Volver al menú
        StartCoroutine(VolverAlMenuDespuesDeTiempo());
    }

    IEnumerator VolverAlMenuDespuesDeTiempo()
    {
        yield return new WaitForSecondsRealtime(tiempoAntesDeSalir);
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
}