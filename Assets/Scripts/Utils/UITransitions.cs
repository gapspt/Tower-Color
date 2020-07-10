using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UITransitions
{
    public const float DefaultTransitionDuration = 0.3f;
    public readonly static AnimationCurve LinearTransition = AnimationCurve.Linear(0, 0, 1, 1);
    public readonly static AnimationCurve EaseInOutTransition = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public readonly static AnimationCurve DefaultTransition = LinearTransition;

    public static Task FadeIn(Graphic image, float duration = DefaultTransitionDuration,
        float delay = 0, AnimationCurve curve = null)
    {
        return Apply(image, value =>
        {
            Color color = image.color;
            color.a = value;
            image.color = color;
        }, duration, delay, curve);
    }

    public static Task FadeOut(Graphic element, float duration = DefaultTransitionDuration,
        float delay = 0, AnimationCurve curve = null)
    {
        return Apply(element, value =>
        {
            Color color = element.color;
            color.a = 1 - value;
            element.color = color;
        }, duration, delay, curve);
    }

    public static Task ScaleIn(UIBehaviour element, float duration = DefaultTransitionDuration,
    float delay = 0, AnimationCurve curve = null)
    {
        return ScaleIn(element.transform, element, duration, delay, curve);
    }
    public static Task ScaleIn(Transform transform, MonoBehaviour source,
        float duration = DefaultTransitionDuration, float delay = 0, AnimationCurve curve = null)
    {
        return Apply(source, value =>
        {
            transform.localScale = new Vector3(value, value, value);
        }, duration, delay, curve);
    }

    public static Task ScaleOut(UIBehaviour element, float duration = DefaultTransitionDuration,
    float delay = 0, AnimationCurve curve = null)
    {
        return ScaleOut(element.transform, element, duration, delay, curve);
    }
    public static Task ScaleOut(Transform transform, MonoBehaviour source,
        float duration = DefaultTransitionDuration, float delay = 0, AnimationCurve curve = null)
    {
        return Apply(source, value =>
        {
            value = 1 - value;
            transform.localScale = new Vector3(value, value, value);
        }, duration, delay, curve);
    }

    public static async Task Apply(MonoBehaviour source, Action<float> action,
        float duration = DefaultTransitionDuration, float delay = 0, AnimationCurve curve = null)
    {
        float startTime = Time.unscaledTime + delay;
        float endTime = startTime + duration;
        if (delay > 0)
        {
            await TaskUtils.WaitForSecondsRealtime(source, delay);
        }
        curve = curve ?? DefaultTransition;
        for (float t = Time.unscaledTime; t < endTime; t = Time.unscaledTime)
        {
            action(curve.Evaluate((t - startTime) / duration));
            await TaskUtils.WaitForNextUpdate(source);
        }
        action(1);
    }
}
