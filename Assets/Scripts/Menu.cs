using Aurinaxtailer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private AudioClip menuLoop;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(menuLoop);
    }

    public void Play()
    {
        SceneManager.LoadScene("Anthony");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
