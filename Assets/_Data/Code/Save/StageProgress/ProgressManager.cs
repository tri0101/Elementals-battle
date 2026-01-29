using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    public PlayerProgress progress;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(progress);
        PlayerPrefs.SetString("player_progress", json);
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("player_progress"))
        {
            string json = PlayerPrefs.GetString("player_progress");
            progress = JsonUtility.FromJson<PlayerProgress>(json);
        }
        else
        {
            progress = new PlayerProgress();
        }
    }
}
