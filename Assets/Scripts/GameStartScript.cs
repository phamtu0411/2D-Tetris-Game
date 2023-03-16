using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartScript : MonoBehaviour
{
    public AudioClip click;
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    public void Play()
    {
        ClickSound();
        SceneManager.LoadScene("Gameplay");
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("QUIT GAME");
    }

    void ClickSound()
    {
        source.PlayOneShot(click);
    }
}
