using UnityEngine;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    public TextMeshProUGUI vidasText;

    public void ActualizarVidas(int vidas)
    {
        vidasText.text = "Vidas: " + vidas;
    }
}
