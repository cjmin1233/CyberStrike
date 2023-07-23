using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public enum SceneType
{
    Start,
    Main,
    Loading,
    Other
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UnityEvent onMainSceneLoaded;
    public UnityEvent onStartSceneLoaded;

    private const string startSceneName = "CyberStrike_Start";
    private const string mainSceneName = "CyberStrike_Main";

    private void Awake()
    {
        if (!Instance) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        onMainSceneLoaded = new UnityEvent();
        onStartSceneLoaded = new UnityEvent();
    }
    public bool LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
            return true;
        }
        return false;
    }
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex.Equals((int)SceneType.Start)) onStartSceneLoaded.Invoke();
        else if (scene.buildIndex.Equals((int)SceneType.Main)) onMainSceneLoaded.Invoke();
    }
}
