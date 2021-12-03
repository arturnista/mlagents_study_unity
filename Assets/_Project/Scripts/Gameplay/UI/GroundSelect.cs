using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSelect : MonoBehaviour
{
    
    private void OnEnable()
    {
        LeanTween.value(gameObject, Vector3.one, Vector3.one * .8f, .5f)
        .setOnUpdate((Vector3 value) => gameObject.transform.localScale = value)
        .setLoopPingPong();
    }

    private void OnDisable()
    {
        LeanTween.cancel(gameObject);
    }

}
