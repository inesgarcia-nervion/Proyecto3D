using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inter : MonoBehaviour
{
    float range = 3.0f;
    public LayerMask Layer;
    [SerializeField] private Transform cameraTransform;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * range, Color.red);

        if (Physics.Raycast(ray, out hit, range, Layer))
        {
            // Debug para ver qué estás mirando
            Debug.Log("Mirando: " + hit.collider.gameObject.name + " | Tag: " + hit.collider.gameObject.tag);

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                Interactuar(hit.collider.gameObject);
            }
        }
    }

    void Interactuar(GameObject go)
    {
        Debug.Log("Intentando interactuar con: " + go.name);

        // Interacción con monedas
        if (go.CompareTag("Coin"))
        {
            Coin coin = go.GetComponent<Coin>();
            if (coin != null)
            {
                coin.Collect();
                Debug.Log("Moneda recogida!");
            }
        }

        // Interacción con baúl
        else if (go.CompareTag("Baul"))
        {
            BaulScript baul = go.GetComponent<BaulScript>();
            if (baul != null)
            {
                Debug.Log("Interactuando con el baúl...");
                baul.Interactuar();
            }
            else
            {
                Debug.LogError("El baúl no tiene el script BaulScript!");
            }
        }
    }
}