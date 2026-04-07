using UnityEngine;
using UnityEngine.UI;

public class UI_PanelPause : MonoBehaviour
{
    [SerializeField] Button buttonPause;
    [SerializeField] Button buttonResume;
    [SerializeField] Button buttonExit;

    [SerializeField] Transform panelPause;

    private void Awake()
    {
        buttonPause.onClick.RemoveAllListeners();
        buttonResume.onClick.RemoveAllListeners();
        buttonExit.onClick.RemoveAllListeners();
        buttonPause.onClick.AddListener(OnPause);
        buttonResume.onClick.AddListener(OnResume);
        buttonExit.onClick.AddListener(OnExit);
    }
    private void OnPause()
    {
        panelPause.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
    private void OnResume()
    {
        panelPause.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
    private void OnExit()
    {
        Time.timeScale = 1f;
        panelPause.gameObject.SetActive(false);
        GameManager.Instance.LoadAdditiveScene(SceneId.MapScene);
        GameManager.Instance.SetCameraActive(true);
        GameManager.Instance.UnLoadAdditiveScene(SceneId.BattleScene);
    }
}