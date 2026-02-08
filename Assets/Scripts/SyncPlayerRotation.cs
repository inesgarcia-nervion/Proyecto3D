using UnityEngine;

public class SyncPlayerRotation : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform modeloVisual; // Referencia al modelo 3D hijo

    void Update()
    {
        // Obtenemos la rotación de la cámara
        float targetRotationY = cameraTransform.eulerAngles.y;

        // Aplicamos rotación al objeto padre (Player)
        transform.rotation = Quaternion.Euler(0, targetRotationY, 0);

        // Si tienes un modelo visual hijo, también rota
        if (modeloVisual != null)
        {
            modeloVisual.rotation = Quaternion.Euler(0, targetRotationY, 0);
        }
    }
}