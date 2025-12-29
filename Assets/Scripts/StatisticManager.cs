using UnityEngine;
using System;
using System.Linq.Expressions;

public class StatisticManager : MonoBehaviour
{
    public static StatisticManager Instance;
    
    [SerializeField] private float quickestTime;
    [SerializeField] private int dayStreak;
    [SerializeField] private string lastDateString;
    
    private DateTime lastDate;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        dayStreak = PlayerPrefs.GetInt("DayStreak", 1);
        quickestTime = PlayerPrefs.GetFloat("QuickestTime", float.MaxValue);
        
        lastDateString = PlayerPrefs.GetString("LastDate", String.Empty);
        
        if (!string.IsNullOrEmpty(lastDateString))
        {
            lastDate = DateTime.Parse(lastDateString);
        }
        else
        {
            lastDate = DateTime.Now.Date;
        }
    }
    
    public void UpdateDayStreak()
    {
        var currentDay = DateTime.Now.Date;
        var difference = currentDay - lastDate;

        if (difference.Days == 1)
        {
            SetDayStreak(dayStreak + 1);
        }
        else if (difference.Days > 1)
        {
            SetDayStreak(1);
        }
    }
    
    public void SetDayStreak(int streak)
    {
        if (streak < 0) return;
        
        dayStreak = streak;
        PlayerPrefs.SetInt("DayStreak", streak);
        PlayerPrefs.SetString("LastDate", lastDateString = DateTime.Now.Date.ToShortDateString());
        PlayerPrefs.Save();
    }

    public int GetDayStreak()
    {
        return dayStreak;
    }

    public void SetQuickestTime(float time)
    {
        if (time >= GetQuickestTime()) return;
        
        quickestTime = time;
        PlayerPrefs.SetFloat("QuickestTime", quickestTime);
        PlayerPrefs.Save();
    }
    
    public float GetQuickestTime()
    {
        return quickestTime;
    }

    [ContextMenu("ResetSaves")]
    public void DeleteSaves()
    {
        PlayerPrefs.DeleteAll();
    }
}
