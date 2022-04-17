using UnityEngine;
using UnityEngine.UI;

public static class UIHelpers
{
    public enum ErrorCode
    {
        InvalidParameters = 0,
        StepSuccess,
        CompleteSuccess
    }

    public static ErrorCode GetClampedLerpPosition(float currentPosition, float targetPosition, float minSpeed, float lerpSpeed, out float finalPosition)
    {
        ErrorCode error = ErrorCode.StepSuccess;

        finalPosition = Mathf.Lerp(currentPosition, targetPosition, Time.deltaTime * lerpSpeed);

        if (Mathf.Abs(targetPosition - finalPosition) < minSpeed)
        {
            finalPosition = targetPosition;
            return ErrorCode.CompleteSuccess;
        }

        float diff = finalPosition - currentPosition;

        if (diff < 0.0f)
            minSpeed = -minSpeed;

        if (Mathf.Abs(diff) < Mathf.Abs(minSpeed))
            finalPosition += minSpeed - diff;

        return error;
    }

    public static float GetRectTop(RectTransform rectTransform)
    {
        return rectTransform.position.y + ((1.0f - rectTransform.pivot.y) * rectTransform.rect.height / CanvasScaleManager.screenScalerRatio.y * rectTransform.localScale.x);
    }

    public static float GetRectBottom(RectTransform rectTransform)
    {
        return rectTransform.position.y - (rectTransform.pivot.y * rectTransform.rect.height / CanvasScaleManager.screenScalerRatio.y * rectTransform.localScale.x);
    }

    public static float GetRectLeft(RectTransform rectTransform)
    {
        return rectTransform.position.x - (rectTransform.pivot.x * rectTransform.rect.width / CanvasScaleManager.screenScalerRatio.x * rectTransform.localScale.x);
    }

    public static float GetRectRight(RectTransform rectTransform)
    {
        return rectTransform.position.x + ((1.0f - rectTransform.pivot.x) * rectTransform.rect.width / CanvasScaleManager.screenScalerRatio.x * rectTransform.localScale.x);
    }

    public static float GetRectCenterWidth(RectTransform rectTransform)
    {
        return rectTransform.position.x - ((rectTransform.pivot.x - 0.5f) * rectTransform.rect.width / CanvasScaleManager.screenScalerRatio.x) * rectTransform.localScale.x;
    }

    public static float GetRectCenterHeight(RectTransform rectTransform)
    {
        return rectTransform.position.y - ((rectTransform.pivot.y - 0.5f) * rectTransform.rect.height / CanvasScaleManager.screenScalerRatio.y) * rectTransform.localScale.y;
    }

    public static float GetRectWidth(RectTransform rectTransform)
    {
        return rectTransform.rect.width / CanvasScaleManager.screenScalerRatio.x * rectTransform.localScale.x;
    }

    public static float GetRectHeight(RectTransform rectTransform)
    {
        return rectTransform.rect.height / CanvasScaleManager.screenScalerRatio.y * rectTransform.localScale.y;
    }

    public static float GetTextWidth(Text text)
    {
        return text.preferredWidth / CanvasScaleManager.screenScalerRatio.x * text.rectTransform.localScale.x;
    }

    public static float GetTextRightSide(Text text)
    {
        RectTransform rectTransform = text.rectTransform;
        float newPosition = rectTransform.position.x;

        switch (text.alignment)
        {
            case TextAnchor.UpperLeft:
            case TextAnchor.MiddleLeft:
            case TextAnchor.LowerLeft:
            {
                newPosition = GetRectLeft(rectTransform) + GetTextWidth(text);
                break;
            }
            case TextAnchor.UpperCenter:
            case TextAnchor.MiddleCenter:
            case TextAnchor.LowerCenter:
            {
                newPosition = GetRectCenterWidth(rectTransform) + GetTextWidth(text) * 0.5f;
                break;
            }
            case TextAnchor.UpperRight:
            case TextAnchor.MiddleRight:
            case TextAnchor.LowerRight:
            {
                newPosition = GetRectRight(rectTransform);
                break;
            }
        }

        return newPosition;
    }

    public static float GetTextLeftSide(Text text)
    {
        RectTransform rectTransform = text.rectTransform;
        float newPosition = rectTransform.position.x;

        switch (text.alignment)
        {
            case TextAnchor.UpperLeft:
            case TextAnchor.MiddleLeft:
            case TextAnchor.LowerLeft:
            {
                newPosition = GetRectLeft(rectTransform);
                break;
            }
            case TextAnchor.UpperCenter:
            case TextAnchor.MiddleCenter:
            case TextAnchor.LowerCenter:
            {
                newPosition = GetRectCenterWidth(rectTransform) - GetTextWidth(text) * 0.5f;
                break;
            }
            case TextAnchor.UpperRight:
            case TextAnchor.MiddleRight:
            case TextAnchor.LowerRight:
            {
                newPosition = GetRectRight(rectTransform) - GetTextWidth(text);
                break;
            }
        }

        return newPosition;
    }

    public static float GetTextCenter(Text text)
    {
        return GetTextLeftSide(text) + GetTextWidth(text) * 0.5f;
    }
}
