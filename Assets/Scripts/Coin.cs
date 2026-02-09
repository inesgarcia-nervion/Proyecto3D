using UnityEngine;

public class Coin : MonoBehaviour
{
    private Renderer rend;
    private Collider col;
    private float velocidad = 100f;

    void Start()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();

        if (rend != null) rend.enabled = true;
        if (col != null) col.enabled = true;
    }

    void Update()
    {
        // Animación de rotación
        transform.Rotate(Vector3.up, velocidad * Time.deltaTime);
    }

    public void Collect()
    {
        // Desactivar la moneda para simular que ha sido recogida
        if (rend != null) rend.enabled = false;
        if (col != null) col.enabled = false;

        CoinManager.Instance.AddCoin();
    }
}