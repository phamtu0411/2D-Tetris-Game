using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public void Replay()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
