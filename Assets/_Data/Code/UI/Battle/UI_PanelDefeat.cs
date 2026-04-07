using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_PanelDefeat : MonoBehaviour
{
    [SerializeField] private GameObject panelDefeat;
    [SerializeField] private Button buttonMenu;

    private Coroutine _playRoutine;

    private void Awake()
    {
        buttonMenu.onClick.RemoveAllListeners();
        buttonMenu.onClick.AddListener(OnClickMenu);
    }
    private void OnEnable()
    {
        if (_playRoutine != null)
            StopCoroutine(_playRoutine);

        _playRoutine = StartCoroutine(CoPlayOpen());
    }

    private void OnDisable()
    {
        if (_playRoutine != null)
        {
            StopCoroutine(_playRoutine);
            _playRoutine = null;
        }
    }

    private IEnumerator CoPlayOpen()
    {
        if (panelDefeat != null)
        {
            panelDefeat.SetActive(true);

            var cgPanel = panelDefeat.GetComponent<CanvasGroup>();
            if (cgPanel == null)
                cgPanel = panelDefeat.AddComponent<CanvasGroup>();

            cgPanel.alpha = 0f;
            cgPanel.interactable = false;
            cgPanel.blocksRaycasts = false;

            yield return FadeCanvasGroup(cgPanel, 0f, 1f, 2f);

            cgPanel.interactable = true;
            cgPanel.blocksRaycasts = true;
        }

        if (buttonMenu != null)
        {
            buttonMenu.gameObject.SetActive(true);

            var cgBtn = buttonMenu.GetComponent<CanvasGroup>();
            if (cgBtn == null)
                cgBtn = buttonMenu.gameObject.AddComponent<CanvasGroup>();

            cgBtn.alpha = 0f;
            cgBtn.interactable = false;
            cgBtn.blocksRaycasts = false;

            yield return FadeCanvasGroup(cgBtn, 0f, 1f, 1f);

            cgBtn.interactable = true;
            cgBtn.blocksRaycasts = true;
        }

        _playRoutine = null;
    }

    private static IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        if (cg == null)
            yield break;

        if (duration <= 0f)
        {
            cg.alpha = to;
            yield break;
        }

        cg.alpha = from;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // keep working even if timeScale changes
            float k = Mathf.Clamp01(t / duration);
            cg.alpha = Mathf.Lerp(from, to, k);
            yield return null;
        }

        cg.alpha = to;
    }
    void OnClickMenu()
    {
        gameObject.SetActive(false);
        GameManager.Instance.LoadAdditiveScene(SceneId.MapScene);
        GameManager.Instance.SetCameraActive(true);
        GameManager.Instance.UnLoadAdditiveScene(SceneId.BattleScene);
    }
}