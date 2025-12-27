using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private bool _isTimerActive;
    private float elapsedTime;
    
    private void Clock()
    {
        if (_isTimerActive)
        {
            elapsedTime += Time.deltaTime;
        
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
        
            _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void StartTimer()
    {
        _isTimerActive = true;
    }

    public void StopTimer()
    {
        _isTimerActive = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0;
    }

    private void Start()
    {
        _isTimerActive = false;
        _timerText = GetComponent<TextMeshProUGUI>();
    }
    
    private void Update()
    {
        Clock();
    }

}
