using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    public string NextSceneName = "main";

    public void OnPlayButtonClick()
    {
        // 加载main场景
        SceneManager.LoadScene(NextSceneName , LoadSceneMode.Single);
    }
} 