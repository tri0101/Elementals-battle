using System.Collections;
using TMPro;
using UnityEngine;

public class UI_LevelUp : MonoBehaviour
{
    public static UI_LevelUp Instance;

    [SerializeField] RectTransform levelUp;
    [SerializeField] TextMeshProUGUI levelUpText;
    [SerializeField] TextMeshProUGUI diamondRewardText;
    [SerializeField] RectTransform circle;
    [SerializeField] RectTransform reward;
    [SerializeField] CanvasGroup canvasGroup;

    [SerializeField] private Transform panelLevelUp;

    private bool canClose = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        panelLevelUp.gameObject.SetActive(false);
    }
    public bool CheckActivePanelLevelUp()
    {
        return panelLevelUp.gameObject.activeSelf;
    }
    public void CallUILevelUp(int amountDiamond)
    {
        panelLevelUp.gameObject.SetActive(true);

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;

        canClose = false;
        PlayerInventory.Instance.AddItem(2, amountDiamond);
        levelUpText.text = AccountManager.Instance.AccountP.level.ToString();
        diamondRewardText.text = amountDiamond.ToString();
        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        // reset
        levelUp.anchoredPosition = new Vector2(-1700, 345);
        circle.localScale = Vector3.zero;
        reward.localScale = Vector3.zero;

        yield return StartCoroutine(MoveLevelUp());
        yield return StartCoroutine(Scale(circle));
        yield return StartCoroutine(Scale(reward));

        // animation xong -> cho phép tắt
        canClose = true;
    }

    IEnumerator MoveLevelUp()
    {
        float time = 0;
        float duration = 0.4f;

        Vector2 start = new Vector2(-1700, 345);
        Vector2 end = new Vector2(0, 345);

        while (time < duration)
        {
            time += Time.deltaTime;
            levelUp.anchoredPosition = Vector2.Lerp(start, end, time / duration);
            yield return null;
        }

        levelUp.anchoredPosition = end;
    }

    IEnumerator Scale(RectTransform obj)
    {
        float time = 0;
        float duration = 0.4f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(0, 1, time / duration);
            obj.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }

        obj.localScale = Vector3.one;
    }

    private void Update()
    {
        if (canClose && Input.GetMouseButtonDown(0))
        {
            canvasGroup.blocksRaycasts = false;
            panelLevelUp.gameObject.SetActive(false);
        }
    }
}