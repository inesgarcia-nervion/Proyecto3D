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
                DoInteraction(hit.collider.gameObject);
            }
        }
        if (Physics.Raycast(ray, out hit, range, Layer))
        {
            // Aquí el jugador está mirando un objeto interactuable
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                CollectCoin(hit.collider.gameObject);
            }
        }
    }

    void DoInteraction(GameObject go)
    {
        // Lógica de interacción con el objeto
        Debug.Log(go.name);
    }

    void CollectCoin(GameObject go)
    {
        Debug.Log(go.name);
        Destroy(go);
    }
}
