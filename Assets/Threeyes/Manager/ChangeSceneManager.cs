using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneManager : MonoBehaviour
{
    public static ChangeSceneManager Instance;
    public KeyCode keyCodeLoadPrevious = KeyCode.None;
    public KeyCode keyCodeLoadNext = KeyCode.None;


    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    void Update()
    {
        if (keyCodeLoadPrevious != KeyCode.None && Input.GetKeyDown(keyCodeLoadPrevious))
        {
            LoadPrevious();
        }
        if (keyCodeLoadNext != KeyCode.None && Input.GetKeyDown(keyCodeLoadNext))
        {
            LoadNext();
        }
    }

    public void LoadPrevious()
    {
        LoadScene(-1);
    }
    public void LoadNext()
    {
        LoadScene(1);
    }

    float lastChangeTime = 0;
    private void LoadScene(int deltaScene)
    {
        if (deltaScene != 0)
        {
            if (Time.time - lastChangeTime < 0.3f)
                return;

            deltaScene = (int)Mathf.Sign(deltaScene);
            Scene curScene = SceneManager.GetActiveScene();
            int nextScene = curScene.buildIndex + deltaScene;
            if (nextScene < 0)
                nextScene += SceneManager.sceneCountInBuildSettings;
            else if (nextScene >= SceneManager.sceneCountInBuildSettings)
                nextScene -= SceneManager.sceneCountInBuildSettings;
            SceneManager.LoadScene(nextScene);

            lastChangeTime = Time.time;
        }
    }

}
