using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public HeroData selectedHero;
    public List<RuneData> selectedRunes = new List<RuneData>();
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void OnClickLoad()
    {
        if (selectedHero == null)
        {
            Debug.LogWarning("Chưa chọn hero!");
            return;
        }

        SceneManager.LoadScene("SampleScene");
    }


    public void LoadAdditiveScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

    }
    public void UnloadAdditiveScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }


  

}
