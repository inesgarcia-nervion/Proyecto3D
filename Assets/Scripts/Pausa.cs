using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class Pausa : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panelPausa; // Panel de pausa asignado en el Inspector

    void Awake()
    {
        // Si no está asignado, intenta encontrar por el nombre común
        if (panelPausa == null)
            panelPausa = GameObject.Find("PanelPausa");

        // Ocultar panel al iniciar
        if (panelPausa != null)
            panelPausa.SetActive(false);

        Time.timeScale = 1f; // Asegurar tiempo normal
    }

    void Update()
    {
        // Detectar Escape para alternar pausa
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
#endif

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    // Alterna pausa y visibilidad del panel
    public void TogglePause()
    {
        if (panelPausa == null) return;

        bool isPaused = Time.timeScale == 0f;
        panelPausa.SetActive(!isPaused);
        Time.timeScale = isPaused ? 1f : 0f;
    }

    // Volver al menú principal
    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuPrincipal");
    }
}
