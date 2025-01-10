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

    //C#ジョブシステムとBurstコンパイラと共に使われるデータ型の一つ
    //ヒープメモリではなく、ネイティブメモリ上に確保される
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
        //第1引数は配列の長さ、第2引数はどのようにメモリを管理するかを指定する
        //Allocator.Temp = 短命な配列（1フレーム内で完結する処理向け）
        //Allocator.TempJob = ジョブシステムのための短命な配列（最大4フレームまで使用可能）
        //Allocator.Persistent = 長期間保持するデータ（複数フレームをまたぐ）今回はこれ
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
        //ジョブシステムにデータを渡す
        var job = new PenLightJob()
        {
            randomSpeeds = _randomSpeeds,
            randomIndices = _randomIndices,
            materialCount = _materialList.Count,
            Random = new Random((uint)Time.frameCount),
        };

        //ジョブを実行し、並列処理をスケジュールする　
        //job.Schedule でジョブの実行をスケジュール
        //第1引数はジョブを実行する回数、第2引数はバッチサイズで、
        //ジョブの処理を分割する単位。大きな数値だと一度に処理し、小さな数値だと細かく分割される。
        JobHandle handle = job.Schedule(_stickAnimList.Count, 64);
        //ジョブの完了を待つ
        handle.Complete();

        //ジョブの結果を反映
        for (int i = 0; i < _stickAnimList.Count; i++)
        {
            _stickAnimList[i].speed = _randomSpeeds[i];
            _stickRendererList[i].material = _materialList[_randomIndices[i]];
        }
    }
    void OnDestroy()
    {
        //NativeArray はメモリを手動で管理する必要があるため、破棄処理を行う
        if (_randomSpeeds.IsCreated) _randomSpeeds.Dispose();
        if (_randomIndices.IsCreated) _randomIndices.Dispose();
    }


    [BurstCompile]
    struct PenLightJob : IJobParallelFor //ジョブシステムで並列処理を行う構造体
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
