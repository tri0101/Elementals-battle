using UnityEngine;

public class DevResetPlayerPrefs : MonoBehaviour
{
#if UNITY_EDITOR
    void Awake()
    {
        PlayerPrefs.DeleteKey("PLAYER_FORMATION_V1");
        PlayerPrefs.Save();
        Debug.Log("DEV: Formation PlayerPrefs reset");
    }
#endif
}
