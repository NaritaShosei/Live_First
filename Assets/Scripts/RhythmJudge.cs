using UnityEngine;
using System.Collections.Generic;

public class RhythmJudge : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;

    //private float _perfectTiming = 0.05f;
    //private float _goodTiming = 0.10f;
    //private float _badTiming = 0.20f;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            CheckHit((float)AudioSettings.dspTime);
        }
    }
    public void CheckHit(float noteTime)
    {
        double currentTime = _gameManager.GetMusicTime();
        double difference = Mathf.Abs((float)(currentTime - noteTime));

        //if (difference <= _perfectTiming)
        //{
        //    Debug.Log("Perfect!!!");
        //}
        //else if (difference <= _goodTiming)
        //{
        //    Debug.Log("Good!");
        //}
        //else if (difference <= _badTiming)
        //{
        //    Debug.Log("Bad...");
        //}
        if (difference <= 0.5f)
        {
            Debug.Log("Nice!");
        }
        else
        {
            Debug.Log("Miss...w");
        }
    }
}
