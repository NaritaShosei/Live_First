using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PenLightManager : MonoBehaviour
{
    [SerializeField] List<Material> _materialList = new();
    [SerializeField] List<GameObject> _stickBlockList = new();

    List<Animator> _stickAnimList = new();
    List<MeshRenderer> _stickRendererList = new();

    [SerializeField, Range(0, 10)] float _interval = 2;
    float _timer;

    void Start()
    {
        StartStick();
        ChangeStick();
    }

    void Update()
    {
        if (Time.time > _timer + _interval)
        {
            _timer = Time.time;
            ChangeStick();
        }
    }

    void StartStick()
    {
        var sticks = GetComponentsInChildren<Animator>();
        foreach (var stick in sticks)
        {
            _stickAnimList.Add(stick);
        }
        for (int i = 0; i < _stickBlockList.Count; i++)
        {
            for (int j = 0; j < _stickBlockList[i].transform.childCount; j++)
            {
                _stickRendererList.Add(_stickBlockList[i].transform.GetChild(j).transform.GetChild(0).GetComponent<MeshRenderer>());
            }
        }
        Debug.Log(_stickAnimList.Count);
    }

    void ChangeStick()
    {
        int index = 0;
        for (int i = 0; i < _stickBlockList.Count; i++)
        {
            var randomIndex = Random.Range(0, _materialList.Count);
            for (int j = 0; j < _stickBlockList[i].transform.childCount; j++)
            {
                _stickRendererList[index].material = _materialList[randomIndex];
                index++;
            }
        }
    }
}
