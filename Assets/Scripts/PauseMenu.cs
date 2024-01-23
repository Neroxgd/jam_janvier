using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPause;

    public void AffichePauseMenu(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        menuPause.SetActive(!menuPause.activeInHierarchy);
    }

    public void Retry()
    {
        SceneManager.LoadScene("Aurelien");
    }

    public void GoMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
