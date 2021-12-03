using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveExplosions : MonoBehaviour
{

    [SerializeField] private float _force = 5f;
    [SerializeField] private float _radius = 10f;
    [Space]
    [SerializeField] private GameObject _effectPrefab;
    
    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Explosion(hit.point);
            }
        }
    }

    private void Explosion(Vector3 position)
    {
        Instantiate(_effectPrefab, position + Vector3.up, Quaternion.identity);
        var colliders = Physics.OverlapSphere(position, _radius, LayerMask.GetMask("Player"));
        foreach (var item in colliders)
        {
            var rigid = item.GetComponent<Rigidbody>();
            if (rigid)
            {
                rigid.AddExplosionForce(_force, position, _radius, 0f, ForceMode.VelocityChange);
            }
        }
    }

}
