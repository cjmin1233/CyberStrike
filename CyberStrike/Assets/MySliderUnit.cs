using UnityEngine;
using UnityEngine.UI;

public class MySliderUnit : MonoBehaviour
{
    RectTransform rect;
    Slider slider;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        slider = GetComponent<Slider>();
    }
    public void SetupRect(float width, float height, Vector2 pos)
    {
        rect.sizeDelta = new(width, height);
        rect.anchoredPosition = pos;
    }
    public void SetValue(float value)
    {
        slider.value = value;
    }
}
