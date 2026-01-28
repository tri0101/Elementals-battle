using UnityEngine;

public class DevResetPlayerPrefs : MonoBehaviour
{
#if UNITY_EDITOR
    void Awake()
    {
        PlayerPrefs.DeleteKey("player_formation_v1");
        PlayerPrefs.Save();
        Debug.Log("DEV: Formation PlayerPrefs reset");
    }
#endif
}
