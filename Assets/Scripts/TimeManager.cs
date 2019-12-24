using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Time _time;

    public static Action NextYear;
    
    private void Start()
    {
        _time.Value = _time.InitialTime;
        StartCoroutine(Tick());
    }

    private IEnumerator Tick()
    {
        yield return new WaitForSeconds(1);

        _time.Value--;

        if (_time.Value == 0)
        {
            NextYear?.Invoke();
            _time.Value = _time.InitialTime;
        }

        StartCoroutine(Tick());
    }
}