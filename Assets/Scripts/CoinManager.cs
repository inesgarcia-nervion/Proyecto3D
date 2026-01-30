using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    public int coinsCollected = 0;
    public TextMeshProUGUI coinText;

    void Awake()
    {
    }

    public void AddCoin()
    {
        coinsCollected++;
        UpdateHUD();
    }

    void UpdateHUD()
    {
        if (coinText != null)
            coinText.text = "Monedas: " + coinsCollected;
    }
}
