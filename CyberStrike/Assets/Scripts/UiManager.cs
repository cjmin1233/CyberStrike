using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private GameObject curCrossHair;
    float originSize;
    [SerializeField] float spreadOffset;

    [SerializeField] private HitImage hitImage;
    [SerializeField] private TextMeshProUGUI magText;

    [SerializeField] private GameObject MainPanel;
    [SerializeField] private GameObject StartPanel;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private MyFadingCG noticeCG;
    [SerializeField] private MySliderUnion playerHealthBar;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    //[SerializeField] private CanvasGroup sprintBarCG;
    //[SerializeField] private Image sprintBarBG;
    //[SerializeField] private Image sprintBar;

    // Input system
    private DefaultInputActions inputAction;

    int popUpCounter = 0;
    private void Awake()
    {
        if (!Instance) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        GameManager.Instance.onMainSceneLoaded.AddListener(MainSceneSetup);
        GameManager.Instance.onStartSceneLoaded.AddListener(StartSceneSetup);
        SetupCrosshair();
    }
    private void Start()
    {
        inputAction = new DefaultInputActions();
        inputAction.UI.Enable();

        inputAction.UI.Cancel.started += OnEscapeTrigger;
    }

    private void OnEscapeTrigger(InputAction.CallbackContext context)
    {
        if (popUpCounter <= 0 && MainPanel.activeSelf)
        {
            popUpCounter++;
            PauseMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void OnPopupUiDisable()
    {
        popUpCounter--;

        if (popUpCounter <= 0 && MainPanel.activeSelf)
        {
            popUpCounter = 0;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    private void SetupCrosshair()
    {
        originSize = curCrossHair.GetComponent<RectTransform>().sizeDelta.x; ;
    }
    public void UpdateCrosshairSpread(float currentSpread, float maxSpread)
    {
        RectTransform rect = curCrossHair.GetComponent<RectTransform>();

        Vector2 curSize = rect.sizeDelta;
        curSize = Vector2.one * (originSize + spreadOffset * currentSpread / maxSpread);
        rect.sizeDelta = curSize;
    }
    public void HitImageUpdate(DamageType damageType)
    {
        hitImage.SetupImages(damageType);
    }
    public void UpdateMag(int magAmmo, int magCapacity)
    {
        magText.text = magAmmo.ToString() + "/" + magCapacity.ToString();
    }
    public void Attempt2LoadNextScene()
    {
        if (GameManager.Instance.LoadNextScene()) print("next scene load complete");
        else print("next scene load failed");
    }
    public void Attempt2QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
    private void StartSceneSetup()
    {
        StartPanel.SetActive(true);
        MainPanel.SetActive(false);
    }
    private void MainSceneSetup()
    {
        MainPanel.SetActive(true);
        StartPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        EnemySpawner.Instance.onDifficultyIncrease += DifficultyNotice;
    }
    private void DifficultyNotice(float difficulty)
    {
        string noticeText = "DIffIculty Increase : " + difficulty.ToString("F1");
        noticeCG.TriggerFadingCG(noticeText);
    }
    public void Notice(string noticeText)
    {
        noticeCG.TriggerFadingCG(noticeText);
    }
    public void UpdatePlayerHealthBar(float health, float maxHealth)
    {
        float value = Mathf.Clamp01(health / maxHealth);
        playerHealthText.text = Mathf.FloorToInt(health).ToString() 
            + "/" + Mathf.FloorToInt(maxHealth).ToString();
        playerHealthBar.SetValue(value);
    }
}
