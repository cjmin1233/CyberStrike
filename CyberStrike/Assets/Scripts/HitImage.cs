using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitImage : MonoBehaviour
{
    CanvasGroup canvasGroup;
    RectTransform rect;
    Image[] images;
    private float originSize;
    private float curSize;
    [SerializeField] float spreadOffset;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        images = GetComponentsInChildren<Image>();

        originSize = rect.sizeDelta.x;
    }
    private void Update()
    {
        canvasGroup.alpha -= 2f * Time.deltaTime;
        curSize = Mathf.Lerp(curSize, 0f, Time.deltaTime);
        //curSize = Mathf.Clamp(curSize - 10f * Time.deltaTime, originSize, originSize + spreadOffset);
        rect.sizeDelta = Vector2.one * curSize;
    }
    public void SetupImages(DamageType damageType)
    {
        Color c = damageType == DamageType.Head ? Color.red : Color.white;

        curSize = damageType == DamageType.Head ? originSize + spreadOffset : originSize;

        canvasGroup.alpha = 1f;
        foreach (Image image in images)
        {
            image.color = c;
        }
    }
}
