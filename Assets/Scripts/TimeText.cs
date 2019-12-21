using TMPro;
using UnityEngine;

public class TimeText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    private int _year = 1;

    private void Awake()
    {
        TimeManager.NextYear += OnNextYear;
    }

    private void OnNextYear()
    {
        _year++;
        _text.text = $"Year {_year}";
    }
    
}