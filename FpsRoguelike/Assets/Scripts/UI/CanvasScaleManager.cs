using UnityEngine;

[DefaultExecutionOrder(-100)]
public class CanvasScaleManager : MonoBehaviour
{
    public static Vector2 screenScalerRatio = Vector2.zero;
    public static float screenRatio = 0.0f;
    public static Vector2 canvasSize = Vector2.zero;
    public static Vector2 screenSize = Vector2.zero;
    public static Vector2 halfScreenSize = Vector2.zero;

    private RectTransform _canvasRect = null;

    void Awake() => SetCanvasSize();

    void SetCanvasSize()
    {
        if (!_canvasRect)
            _canvasRect = (RectTransform)transform;

        canvasSize = _canvasRect.sizeDelta;
        screenSize = new Vector2(Screen.width, Screen.height);

        screenScalerRatio = canvasSize / screenSize;
        screenRatio = (float)Screen.width / Screen.height;

        halfScreenSize = screenSize / 2;
    }

    void OnRectTransformDimensionsChange() => SetCanvasSize();
}