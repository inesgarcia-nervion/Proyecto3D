using UnityEngine;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class Pausa : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private GameObject panelPausa;

    // Intent: toggle pause when player presses Escape (or the Cancel input)
    void Awake()
    {
        // If the panel was not assigned in the Inspector, try to find it by common names or tag to avoid silent failures.
        if (panelPausa == null)
        {
            // Try a few common names (adjust to match your scene hierarchy)
            panelPausa = GameObject.Find("PanelPausa") ?? GameObject.Find("Panel Pausa") ?? GameObject.Find("PausePanel");

            // Try finding by a tag if you set one in the inspector (optional)
            if (panelPausa == null)
            {
                var byTag = GameObject.FindWithTag("PausePanel");
                if (byTag != null) panelPausa = byTag;
            }

            if (panelPausa == null)
            {
                Debug.LogWarning("Pausa: 'panelPausa' no está asignado y no se encontró ningún objeto con nombres comunes. Asigne el panel en el Inspector.");
            }
        }

        // Ensure the panel is hidden at start
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }

        // Ensure the game is running at normal time on start
        Time.timeScale = 1f;
    }

    // Cuando el jugador presiona la tecla de esc, se activa o desactiva el estado de pausa del juego.
    void Update()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        // Use the new Input System if it's enabled and the legacy input manager is disabled
        var keyboard = Keyboard.current;
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }

        // Optionally support a "Cancel" style button on gamepad (adjust as needed)
        var gamepad = Gamepad.current;
        if (gamepad != null && (gamepad.startButton.wasPressedThisFrame || gamepad.buttonSouth.wasPressedThisFrame))
        {
            TogglePause();
        }
#else
        // Legacy input manager
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel"))
        {
            TogglePause();
        }
#endif
    }

    // Toggle pause state and panel
    public void TogglePause()
    {
        if (panelPausa == null)
        {
            Debug.LogWarning("Pausa: No se puede cambiar el estado de pausa porque 'panelPausa' es null.");
            return;
        }

        bool isPaused = Time.timeScale == 0f;

        if (!isPaused)
        {
            panelPausa.SetActive(true);
            Time.timeScale = 0f; // Pausa el juego
        }
        else
        {
            panelPausa.SetActive(false);
            Time.timeScale = 1f; // Reanuda el juego
        }
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MenuPrincipal");
    }

}
