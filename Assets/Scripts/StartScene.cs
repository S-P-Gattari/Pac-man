using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneController : MonoBehaviour
{
    public Button level1Button;
    public Button level2Button;

    void Start()
    {
        level1Button.onClick.AddListener(() => LoadLevel("Level01"));

        level2Button.onClick.AddListener(() => LoadLevel("InnovationScene"));
    }

    void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
