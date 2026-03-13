using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public DateTime lastTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTime();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadTime()
    {
        if (PlayerPrefs.HasKey("LastTime"))
        {
            lastTime = DateTime.Parse(PlayerPrefs.GetString("LastTime"));
        }
        else
        {
            lastTime = DateTime.Now;
        }
    }

    public int GetSecondsPassed()
    {
        return (int)(DateTime.Now - lastTime).TotalSeconds;
    }

    public void AdvanceTime(int seconds)
    {
        lastTime = lastTime.AddSeconds(seconds);

        PlayerPrefs.SetString("LastTime", lastTime.ToString());
        PlayerPrefs.Save();
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("LastTime", lastTime.ToString());
        PlayerPrefs.Save();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            lastTime = DateTime.Now.AddMinutes(-10);
            Debug.Log("Test offline 10 phút");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            lastTime = DateTime.Now.AddHours(-1);
            Debug.Log("Test offline 1 giờ");
        }
    }
}