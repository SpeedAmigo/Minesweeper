using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private bool isTimerActive;
    
    private float elapsedTime;
    
    private void Clock()
    {
        if (isTimerActive)
        {
            elapsedTime += Time.deltaTime;
        
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
        
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void StartTimer()
    {
        isTimerActive = true;
    }

    public void StopTimer()
    {
        isTimerActive = false;
    }

    public void ResetTimer()
    {
        elapsedTime = 0;
        timerText.text = "00:00";
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    private void Start()
    {
        isTimerActive = false;
        //timerText = GetComponent<TextMeshProUGUI>();
    }
    
    private void Update()
    {
        Clock();
    }
}
