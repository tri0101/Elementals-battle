using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum SceneId
{
    MainScene = 0,
    GachaSceneManager = 1,
    HeroManagerScene= 2,
    BattleScene = 3,
    HeroUpgradeScene = 4,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public HeroData selectedHero;
    public List<RuneData> selectedRunes = new List<RuneData>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
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


    public void LoadAdditiveScene(SceneId sceneLoad)
    {
        SceneManager.LoadScene(sceneLoad.ToString(), LoadSceneMode.Additive);

    }
    public void UnLoadAdditiveScene(SceneId sceneLoad)
    {
        SceneManager.UnloadSceneAsync(sceneLoad.ToString());
    }




}
