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
            Debug.Log("El juego ya terminó");
            return;
        }

        GuardarMonedas();
    }

    void GuardarMonedas()
    {
        if (CoinManager.Instance == null)
        {
            Debug.LogError("CoinManager.Instance es null!");
            return;
        }

        // Verificar si el jugador tiene monedas
        if (CoinManager.Instance.coinsCollected <= 0)
        {
            Debug.Log("No tienes monedas para guardar. Monedas actuales: " + CoinManager.Instance.coinsCollected);
            return;
        }

        // Transferir una moneda del jugador al baúl
        monedasGuardadas += 1;
        CoinManager.Instance.coinsCollected -= 1;

        // Actualizar ambos HUD
        CoinManager.Instance.UpdateHUD();
        UpdateHUD();

        Debug.Log("¡Moneda guardada! Baúl: " + monedasGuardadas + "/" + monedasParaGanar + " | Jugador: " + CoinManager.Instance.coinsCollected);

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
        else
            Debug.LogWarning("baulText no está asignado en el Inspector!");
    }

    void Victoria()
    {
        Debug.Log("¡VICTORIA!");
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