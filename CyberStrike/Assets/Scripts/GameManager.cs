using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

public enum SceneType
{
    Start,
    Main,
    Loading,
    Other
}
public enum GameState
{
    Running,
    Paused
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public UnityEvent onMainSceneLoaded;
    public UnityEvent onStartSceneLoaded;
    public UnityEvent onGameOver;
    public event Action<bool> onGamePaused;

    private Coroutine mainSceneSetupProcess;
    private Coroutine mainSceneProcess;
    public float gameTimer { get; private set; }
    public float gameScore { get; private set; }
    public bool isGameOver { get; private set; }
    public GameState gameState { get; private set; }
    private void Awake()
    {
        if (!Instance) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        onMainSceneLoaded = new UnityEvent();
        onStartSceneLoaded = new UnityEvent();
        onGameOver = new UnityEvent();
        onGameOver.AddListener(SaveData);
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
    public bool LoadScene(SceneType sceneType)
    {
        int nextSceneIndex = (int)sceneType;
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
        if (scene.buildIndex.Equals((int)SceneType.Start))
        {
            gameState = GameState.Running;
            onStartSceneLoaded.Invoke();
        }
        else if (scene.buildIndex.Equals((int)SceneType.Main))
        {
            gameState = GameState.Running;
            onMainSceneLoaded.Invoke();
            mainSceneSetupProcess = StartCoroutine(MainSceneSetupProcess());
        }
    }
    private IEnumerator MainSceneSetupProcess()
    {
        gameTimer = 0f;
        gameScore = 0f;
        isGameOver = false;

        // 적들이 몰려옵니다. 준비하세요....
        UiManager.Instance.Notice("Get Ready...");
        UiManager.Instance.UpdateGameScore(gameScore);

        yield return new WaitForSeconds(1f);

        // 3,2,1...start
        UiManager.Instance.Notice("3");
        yield return new WaitForSeconds(1f);
        UiManager.Instance.Notice("2");
        yield return new WaitForSeconds(1f);
        UiManager.Instance.Notice("1");
        yield return new WaitForSeconds(1f);
        UiManager.Instance.Notice("Start");

        EnemySpawner.Instance.StartEnemySpawn();
        ItemSpawner.Instance.StartItemSpawn();

        PlayerHealth playerHealth = PlayerControllerFPS.Instance.GetComponent<PlayerHealth>();
        playerHealth.onDeath += OnPlayerDeath;

        mainSceneProcess = StartCoroutine(MainSceneProcess());
    }
    private IEnumerator MainSceneProcess()
    {
        while (!isGameOver)
        {
            gameTimer += Time.deltaTime;
            gameScore += Time.deltaTime;
            UiManager.Instance.UpdateGameScore(gameScore);
            yield return null;
        }
        onGameOver.Invoke();
    }
    private void OnPlayerDeath()
    {
        isGameOver = true;
    }
    public void AddScore(float value)
    {
        gameScore += value;
    }
    public void PauseGame(bool isPaused)
    {
        onGamePaused(isPaused);
        gameState = isPaused ? GameState.Paused : GameState.Running;
        
        Time.timeScale = isPaused ? 0f : 1f;
    }
    public void SaveData()
    {
        float bestScore = PlayerPrefs.GetFloat("BestScore");
        if (bestScore < gameScore)
        {
            bestScore = gameScore;
            PlayerPrefs.SetFloat("BestScore", bestScore);
        }
    }
}
