using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    public int coinsCollected = 0;
    public TextMeshProUGUI coinText; 

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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
