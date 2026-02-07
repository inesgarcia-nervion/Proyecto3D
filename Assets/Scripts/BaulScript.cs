using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class BaulScript : MonoBehaviour
{
    public int monedasGuardadas = 0;
    public int monedasParaGanar = 15; // ← Cantidad necesaria para ganar
    public TextMeshProUGUI baulText;
    public float interactRange = 3f;
    public Transform player;

    [Header("Victoria")]
    public GameObject panelVictoria; // ← Panel de victoria (asignar en el Inspector)
    public float tiempoAntesDeSalir = 3f; // Tiempo antes de volver al menú

    private bool juegoTerminado = false;

    Renderer rend;
    Collider col;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();

        // Asegurar que el panel de victoria esté oculto al inicio
        if (panelVictoria != null)
            panelVictoria.SetActive(false);
    }

    void Start()
    {
        UpdateHUD();
    }

    void Update()
    {
        if (player == null || juegoTerminado) return;

        // Comprobar si el jugador está cerca y pulsa E
        if (Vector3.Distance(player.position, transform.position) <= interactRange)
        {
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                GuardarMonedas();
            }
        }
    }

    void GuardarMonedas()
    {
        if (CoinManager.Instance == null) return;

        // Solo transferir si el jugador tiene al menos una moneda
        if (CoinManager.Instance.coinsCollected <= 0) return;

        monedasGuardadas += 1;
        CoinManager.Instance.coinsCollected -= 1;
        CoinManager.Instance.UpdateHUD();
        UpdateHUD();

        // ¡COMPROBAR VICTORIA!
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
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.enabled = false;
        }

        // Mostrar panel de victoria
        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }

        // Detener el tiempo (opcional, para congelar el juego)
        Time.timeScale = 0f;

        // Volver al menú después de unos segundos
        StartCoroutine(VolverAlMenuDespuesDeTiempo());
    }

    IEnumerator VolverAlMenuDespuesDeTiempo()
    {
        // Esperar usando tiempo real (no afectado por timeScale)
        yield return new WaitForSecondsRealtime(tiempoAntesDeSalir);

        // Restaurar el tiempo antes de cambiar de escena
        Time.timeScale = 1f;

        // Cargar menú principal
        SceneManager.LoadScene("MenuPrincipal");
    }
}