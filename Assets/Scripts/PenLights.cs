using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Random = Unity.Mathematics.Random;
using System;

public class PenLights : MonoBehaviour
{
    [SerializeField] List<Material> _materialList = new();

    List<Animator> _stickAnimList = new();
    List<MeshRenderer> _stickRendererList = new();

    //C#�W���u�V�X�e����Burst�R���p�C���Ƌ��Ɏg����f�[�^�^�̈��
    //�q�[�v�������ł͂Ȃ��A�l�C�e�B�u��������Ɋm�ۂ����
    NativeArray<float> _randomSpeeds;
    NativeArray<int> _randomIndices;

    [SerializeField, Range(0, 10)] float _interval = 2;
    float _timer;

    void Start()
    {
        _timer = Time.time;
        StartStick();
        ChangeStick();
    }

    void StartStick()
    {
        var sticks = GetComponentsInChildren<Animator>();
        foreach (var stick in sticks)
        {
            _stickAnimList.Add(stick);
            _stickRendererList.Add(stick.gameObject.GetComponentInChildren<MeshRenderer>());
        }
        Debug.Log(_stickAnimList.Count);
        //��1�����͔z��̒����A��2�����͂ǂ̂悤�Ƀ��������Ǘ����邩���w�肷��
        //Allocator.Temp = �Z���Ȕz��i1�t���[�����Ŋ������鏈�������j
        //Allocator.TempJob = �W���u�V�X�e���̂��߂̒Z���Ȕz��i�ő�4�t���[���܂Ŏg�p�\�j
        //Allocator.Persistent = �����ԕێ�����f�[�^�i�����t���[�����܂����j����͂���
        _randomSpeeds = new NativeArray<float>(_stickAnimList.Count, Allocator.Persistent);
        _randomIndices = new NativeArray<int>(_stickAnimList.Count, Allocator.Persistent);

    }

    void Update()
    {
        if (Time.time > _timer + _interval)
        {
            _timer = Time.time;
            ChangeStick();
        }
    }
    void ChangeStick()
    {
        //�W���u�V�X�e���Ƀf�[�^��n��
        var job = new PenLightJob()
        {
            randomSpeeds = _randomSpeeds,
            randomIndices = _randomIndices,
            materialCount = _materialList.Count,
            Random = new Random((uint)Time.frameCount),
        };

        //�W���u�����s���A���񏈗����X�P�W���[������@
        //job.Schedule �ŃW���u�̎��s���X�P�W���[��
        //��1�����̓W���u�����s����񐔁A��2�����̓o�b�`�T�C�Y�ŁA
        //�W���u�̏����𕪊�����P�ʁB�傫�Ȑ��l���ƈ�x�ɏ������A�����Ȑ��l���ƍׂ������������B
        JobHandle handle = job.Schedule(_stickAnimList.Count, 64);
        //�W���u�̊�����҂�
        handle.Complete();

        //�W���u�̌��ʂ𔽉f
        for (int i = 0; i < _stickAnimList.Count; i++)
        {
            _stickAnimList[i].speed = _randomSpeeds[i];
            _stickRendererList[i].material = _materialList[_randomIndices[i]];
        }
    }
    void OnDestroy()
    {
        //NativeArray �̓��������蓮�ŊǗ�����K�v�����邽�߁A�j���������s��
        if (_randomSpeeds.IsCreated) _randomSpeeds.Dispose();
        if (_randomIndices.IsCreated) _randomIndices.Dispose();
    }


    [BurstCompile]
    struct PenLightJob : IJobParallelFor //�W���u�V�X�e���ŕ��񏈗����s���\����
    {
        public NativeArray<float> randomSpeeds;
        public NativeArray<int> randomIndices;
        public int materialCount;
        public Random Random;
        public void Execute(int index)
        {
            randomSpeeds[index] = 1;
            //randomSpeeds[index] = random.NextFloat(0.99f, 1);
            randomIndices[index] = Random.NextInt(0, materialCount);
        }
    }
}
