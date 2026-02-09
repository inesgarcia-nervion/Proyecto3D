using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    // Todo acerca del tiempo de respawn se ha comentado ya que no creo que lo utilice en el proyecto final, pero lo dejo por si acaso
    // public float respawnTime = 5f;       // Tiempo para reaparecer
    private Vector3 initialPosition;
    private Renderer rend;
    private Collider col;
    private float velocidad = 100f;

    void Start()
    {
        // initialPosition = transform.position;  // Guardaba posición inicial para respawn
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider>();
        if (rend != null) rend.enabled = true;
        if (col != null) col.enabled = true;
    }

    void Update()
    {
        // Rotar la moneda para un efecto visual
        transform.Rotate(Vector3.up, velocidad * Time.deltaTime);
    }

    public void Collect()
    {
        // Desactivar visual y collider
        if (rend != null) rend.enabled = false;
        if (col != null) col.enabled = false;

        // Sumar al contador
        CoinManager.Instance.AddCoin();

        // --- RESPAWN (comentado) ---
        // StartCoroutine(Respawn());
    }

    // --- CORRUTINA DE RESPAWN (comentada) ---
    /*
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        transform.position = initialPosition;
        if (rend != null) rend.enabled = true;
        if (col != null) col.enabled = true;
    }
    */
}