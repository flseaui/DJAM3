using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    private Slider _slider;

    [SerializeField]
    private Time _time;
    
    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.maxValue = _time.InitialTime;
    }

    private void Update()
    {
        _slider.value = _time.Value;
    }
}