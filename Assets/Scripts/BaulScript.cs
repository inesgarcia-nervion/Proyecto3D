using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaulScript : MonoBehaviour
{
    public int monedasGuardadas = 0;          // Monedas dentro del baúl
    public TextMeshProUGUI baulText;          
    public float interactRange = 3f;          // Distancia para interactuar
    public Transform player;                  // Referencia al jugador

    Renderer rend;
    Collider col;

    void Awake()
    {
        // Guardar referencias seguras
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
    }

    void Start()
    {
        UpdateHUD();
    }

    void Update()
    {
        if (player == null) return; 

        // Comprobamos si el jugador está cerca y pulsa E
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
        if (CoinManager.Instance == null)
        {
            return;
        }

        // Solo transferir una moneda por pulsación si el jugador tiene al menos una
        if (CoinManager.Instance.coinsCollected <= 0)
        {
            return;
        }

        monedasGuardadas += 1; // sumar una al baúl
        CoinManager.Instance.coinsCollected -= 1; // quitar una al jugador
        CoinManager.Instance.UpdateHUD();
        UpdateHUD();
    }

    void UpdateHUD()
    {
        if (baulText != null)
            baulText.text = "Monedas en el baúl: " + monedasGuardadas;
    }
}