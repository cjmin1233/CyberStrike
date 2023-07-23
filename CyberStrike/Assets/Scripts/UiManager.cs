using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [SerializeField] private GameObject curCrossHair;
    float originSize;
    [SerializeField] float spreadOffset;

    [SerializeField] private HitImage hitImage;
    [SerializeField] private TextMeshProUGUI magText;
    private void Awake()
    {
        if (!Instance) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        SetupCrosshair();
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
}
