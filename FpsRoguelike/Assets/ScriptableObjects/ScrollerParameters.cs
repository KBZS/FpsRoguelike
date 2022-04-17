using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScrollerParameters")]
public class ScrollerParameters : ScriptableObject
{
    public float scrollSpeed = 5.0f;
    public float scrollSensitivity = 2.0f;
    public float scrollDelayPostInit = 1.0f;
    public float dragAmountBeforeScroll = 20.0f;
    public float minimumScrollSpeed = 0.03f;
}
