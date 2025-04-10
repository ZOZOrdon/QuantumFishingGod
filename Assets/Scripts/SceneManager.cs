using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public string NextSceneName = "main";
    [SerializeField] private Image fadePanel; // 用于淡入淡出的面板
    [SerializeField] private float fadeTime = 2.0f; // 淡入淡出时间

    private void Start()
    {
        // 确保游戏开始时面板是透明的
        if (fadePanel != null)
        {
            Color color = fadePanel.color;
            color.a = 0f;
            fadePanel.color = color;
        }
    }

    public void OnPlayButtonClick()
    {
        Debug.Log("开始场景切换");
        StartCoroutine(LoadSceneCoroutine(1)); // 这里可以根据需要传入场景索引
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        if (fadePanel == null)
        {
            Debug.LogError("淡入淡出面板未设置！");
            yield break;
        }

        // 设置不要在加载时销毁
        DontDestroyOnLoad(fadePanel.gameObject);

        // 执行淡入（从透明到黑色）
        yield return StartCoroutine(FadeToBlack());
        Debug.Log("淡入完成");

        // 加载新场景
        Debug.Log("开始加载新场景");
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneIndex);
        
        // 等待场景加载完成
        while (!async.isDone)
        {
            yield return null;
        }
        
        // Debug.Log("场景加载完成，开始淡出");
        
        // // 执行淡出（从黑色到透明）
        // yield return StartCoroutine(FadeFromBlack());
        // Debug.Log("淡出完成");
        
        // 销毁面板
        Destroy(fadePanel.gameObject);
    }

    private IEnumerator FadeToBlack()
    {
        if (fadePanel == null) yield break;

        float elapsedTime = 0f;
        Color color = fadePanel.color;
        color.a = 0f; // 确保开始时是透明的
        fadePanel.color = color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            fadePanel.color = color;
            yield return null;
        }

        // 确保最终完全不透明
        color.a = 1f;
        fadePanel.color = color;
    }

    private IEnumerator FadeFromBlack()
    {
        if (fadePanel == null) yield break;

        float elapsedTime = 0f;
        Color color = fadePanel.color;
        color.a = 1f; // 确保开始时是不透明的
        fadePanel.color = color;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            fadePanel.color = color;
            yield return null;
        }

        // 确保最终完全透明
        color.a = 0f;
        fadePanel.color = color;
    }
} 