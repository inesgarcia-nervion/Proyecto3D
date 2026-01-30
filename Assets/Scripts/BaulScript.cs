using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaulScript : MonoBehaviour
{
    public int monedasGuardadas = 0;          // Monedas dentro del baúl
    public TextMeshProUGUI baulText;          // HUD del baúl
    public float interactRange = 3f;          // Distancia para interactuar
    public Transform player;                  // Referencia al jugador

    void Start()
    {
        // Buscamos el Text en la UI
        baulText = GameObject.Find("Canvas").GetComponentsInChildren<TextMeshProUGUI>()[1];
        UpdateHUD();
    }

    void Update()
    {
        // Comprobamos si el jugador está cerca y pulsa E
        if (Vector3.Distance(player.position, transform.position) <= interactRange)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                GuardarMonedas();
            }
        }
    }

    void GuardarMonedas()
    {
        // Guardar todas las monedas que el jugador tiene
        monedasGuardadas += CoinManager.Instance.coinsCollected;
        CoinManager.Instance.coinsCollected = 0; // Reseteamos monedas del jugador
        CoinManager.Instance.UpdateHUD();
        UpdateHUD();
    }

    void UpdateHUD()
    {
        if (baulText != null)
            baulText.text = "Monedas en el baúl: " + monedasGuardadas;
    }
}
