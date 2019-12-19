using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class FixCanvasTool : MonoBehaviour
{
    void Awake()
    {
        FixResolution();
    }

    public void FixResolution()
    {
        CanvasScaler scaler = GetComponent<CanvasScaler>();

        float sWToH = scaler.referenceResolution.x * 1.0f / scaler.referenceResolution.y;
        float vWToH = Screen.width * 1.0f / Screen.height;
        if (sWToH > vWToH)
        {
            //匹配宽
            scaler.matchWidthOrHeight = 0;
        }
        else
        {
            //匹配高
            scaler.matchWidthOrHeight = 1;
        }
    }
}