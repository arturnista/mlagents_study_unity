using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{

    private static ScreenShake s_Instance;

    private static bool _enabled = true;

    public static void Shake(float strength, float time)
    {
        if (!_enabled) return;
        s_Instance.StartCoroutine(s_Instance.ScreenShakeCoroutine(strength, time));
    }

    public static void Stop()
    {
        s_Instance.StopAllCoroutines();
    }

    public static void Enable()
    {
        _enabled = true;
    }

    public static void Disable()
    {
        _enabled = false;
        Stop();
    }

    private void Awake()
    {
        s_Instance = this;
    }
    
    private IEnumerator ScreenShakeCoroutine(float strength, float time)
    {
        float currentTime = 0f;
        while (currentTime < time)
        {
            currentTime += Time.deltaTime;
            transform.localPosition = Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.localPosition = Vector3.zero;
    }

}
