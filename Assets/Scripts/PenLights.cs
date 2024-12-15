using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PenLights : MonoBehaviour
{
    [SerializeField] List<Material> _materialList = new();
    [SerializeField] GameObject _stick;
    [SerializeField] Vector3 _stickPos = new Vector3(15, 0, 30);
    void Start()
    {
        StartStick();
    }

    async void StartStick()
    {
        for (float i = 0; i < _stickPos.x; i += 0.5f)
        {
            for (float j = 0; j < _stickPos.z; j += 0.5f)
            {
                await InstantiateAsync(_stick, transform.position + new Vector3(i, 0, j), Quaternion.identity);
            }
        }
    }

    void Update()
    {

    }
}
