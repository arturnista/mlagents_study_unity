using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    [SerializeField]
    private Transform _object = default;

    private Camera _camera;

    private void Awake()
    {
        if (_object == null) _object = transform;
        _camera = Camera.main;
    }
 
    //Orient the camera after all movement is completed this frame to avoid jittering
    private void LateUpdate()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
            return;
        }
        
        Vector3 viewDirection = new Vector3(_camera.transform.forward.x, 0, _camera.transform.forward.z);
        _object.LookAt(_object.position + viewDirection);
    }
}