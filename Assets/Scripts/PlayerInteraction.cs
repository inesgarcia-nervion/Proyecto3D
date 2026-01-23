using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class Inter : MonoBehaviour
{
    float range = 2.0f; // Distancia máxima de interacción
    public LayerMask Layer; // Capa de objetos interactuables

    void Update()
    {
        // 1. Definimos el rayo desde la posición de la cámara hacia adelante
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Visualización en el editor (solo visible en la escena)
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red);

        // 2. Comprobamos si el rayo choca con algo en la capa seleccionada
        if (Physics.Raycast(ray, out hit, range, Layer))
        {
            // Aquí el jugador está mirando un objeto interactuable
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                CollectCoin(hit.collider.gameObject);
            }
          
        }
        
    }

    void CollectCoin(GameObject go)
    {
        // Lógica de interacción con el objeto
        Coin coin = go.GetComponent<Coin>();
        if (coin != null)
        {
            coin.Collect(); // Toda la lógica está en el script Coin
        }
    }
}
