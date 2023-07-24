using UnityEngine;
using UnityEngine.UI;

public class MySliderUnion : MonoBehaviour
{
    [SerializeField] private float Width;
    [SerializeField] private float Height;
    [SerializeField] private int unitCount;
    [SerializeField] private GameObject sliderUnitPrefab;
    MySliderUnit[] sliderUnits;

    float unitWidth;
    [Range(0f, 1f), SerializeField] private float R = 0f;
    private void Awake()
    {
        sliderUnits = new MySliderUnit[unitCount];

        if (unitCount <= 0) return;
        unitWidth = (int)Width / unitCount;

        float startPos = -Width / 2f + unitWidth / 2f;
        for (int i = 0; i < sliderUnits.Length; i++)
        {
            var instance = Instantiate(sliderUnitPrefab);
            instance.transform.SetParent(transform);
            sliderUnits[i] = instance.GetComponent<MySliderUnit>();

            float x = startPos + i * unitWidth;
            sliderUnits[i].SetupRect(unitWidth, Height, new(x, 0f));
        }
    }
    private void Update()
    {
        SetValue(R);
    }
    public void SetValue(float value)
    {
        int NumOfUnit = (int)(value * unitCount);

        // 100%�� ĭ ä���
        for(int i = 0; i < NumOfUnit; i++)
        {
            sliderUnits[i].SetValue(1f);
        }
        // ��� 1�� ä���
        if (NumOfUnit < sliderUnits.Length)
        {
            float remain = value * unitCount - NumOfUnit;
            sliderUnits[NumOfUnit].SetValue(remain);
        }
        // �޺κ� 0���� ����
        for(int i = NumOfUnit + 1; i < sliderUnits.Length; i++)
        {
            sliderUnits[i].SetValue(0f);
        }
    }
}
