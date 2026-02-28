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
    InventoryScene = 5,
    MapScene = 6,
    PersistentScene = 7,
    StoreScene = 8,

}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public HeroData selectedHero;
    public List<RuneData> selectedRunes = new List<RuneData>();
    public Transform cameraMain;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAdditiveScene(SceneId.MainScene);
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

    public void SetCameraActive(bool isActive)
    {
        cameraMain.gameObject.SetActive(isActive);
    }
    public void LoadAdditiveScene(SceneId sceneLoad)
    {
        SceneManager.LoadScene(sceneLoad.ToString(), LoadSceneMode.Additive);
        if(sceneLoad == SceneId.BattleScene)
        {
            SetCameraActive(false);
        }
    }
    public void UnLoadAdditiveScene(SceneId sceneLoad)
    {
        SceneManager.UnloadSceneAsync(sceneLoad.ToString());
    }




}
