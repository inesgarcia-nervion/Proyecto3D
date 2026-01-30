using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    public int coinsCollected = 0;
    public TextMeshProUGUI coinText;

    void Awake()
    {
        coinText = GameObject.Find("Canvas").GetComponentsInChildren<TextMeshProUGUI>()[0];
        Instance = this;
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
