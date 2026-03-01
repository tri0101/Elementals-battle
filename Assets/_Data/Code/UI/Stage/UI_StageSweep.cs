using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StageSweep : MonoBehaviour
{
    [Header("Transform")]
    [SerializeField] private GameObject listSweep;
    [SerializeField] private Transform panelListSweep;
    [SerializeField] private Button buttonAgain;
    [SerializeField] private Button buttonOk;
    [SerializeField] private ScrollRect scrollRect;

    int sweepTimes; //số lần quét lý thuyết (1 hoặc 10)
    int canSweepTimes; //số lần quét thực tế 
    List<GameObject> sweepItems = new List<GameObject>();
    Coroutine showCoroutine;
    Coroutine scrollCoroutine;

    bool isSequentialRunning = false;

    private void Awake()
    {
        buttonOk.onClick.AddListener(OnClickOK);
    }

    private void OnEnable()
    {
        ClearListSweeps();

        if (sweepTimes == 1)
        {
            buttonAgain.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Clear x1";
            buttonAgain.onClick.RemoveAllListeners();
            buttonAgain.onClick.AddListener(() => OnClickOne());

            //OnClickOne();
        }
        else
        {
            buttonAgain.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Clear x10";
            buttonAgain.onClick.RemoveAllListeners();
            buttonAgain.onClick.AddListener(()=> OnClickTen());

            //OnClickTen();
        }
    }
    
    void Update()
    {
        if (isSequentialRunning && Input.GetMouseButtonDown(0))
        {
            SkipAll();
        }
    }

    void OnClickOK()
    {
        gameObject.SetActive(false);
    }

    void ClearListSweeps()
    {
        foreach (Transform child in panelListSweep)
        {
            Destroy(child.gameObject);
        }

        sweepItems.Clear();
    }

    public int GetTime(int times) // times là số lần lý thuyết ,còn sweepTimes là số lần thực tế sau khi check stamina
    {
        if (times == 1)
        {
            if (PlayerInventory.Instance.GetItemQuantity(3) < StageContext.selectedStage.staminaCost)
            {
                return 0;
            }
            else
            {
                gameObject.SetActive(true);

                PlayerInventory.Instance.ConsumeItem(3, StageContext.selectedStage.staminaCost);
                return 1;
               
            }
        }
        else if (times == 10)
        {
            int SweepBasedOnStamina = 0;
            if (PlayerInventory.Instance.GetItemQuantity(3) < StageContext.selectedStage.staminaCost)
            {
                return 0;
            }
            if (PlayerInventory.Instance.GetItemQuantity(3) >= StageContext.selectedStage.staminaCost * 10)
            {
                SweepBasedOnStamina = 10; // có đủ stamina để quét 10 lần
                PlayerInventory.Instance.ConsumeItem(3, StageContext.selectedStage.staminaCost * 10);
                gameObject.SetActive(true);
                return 10;
                
            }
            else
            {
                SweepBasedOnStamina = PlayerInventory.Instance.GetItemQuantity(3) / StageContext.selectedStage.staminaCost; // số lần có thể quét dựa trên stamina hiện tại
                PlayerInventory.Instance.ConsumeItem(3, SweepBasedOnStamina * StageContext.selectedStage.staminaCost);
                gameObject.SetActive(true);
                return SweepBasedOnStamina;

            }
        }
        return 0;
    }
    public void SetTimes(int times)
    {
        sweepTimes = times;
        
    }

    void SetButtonsActive(bool value)
    {
        buttonAgain.gameObject.SetActive(value);
        buttonOk.gameObject.SetActive(value);
    }


    public void OnClickOne()
    {
        if(GetTime(1) == 0)
        {
            UI_ShowResource.Instance.UI_Exchange.ShowPanelBuyStamina();
            return;
        }
        
        ClearListSweeps();

        var go = Instantiate(listSweep, panelListSweep);
        var ui = go.GetComponent<UI_ListSweep>();

        ui.SetText(1);
        ui.SetItemDrop(DropSystem.RollDrops());

        go.SetActive(true);

        ScrollToBottom();
    }

  

    public void OnClickTen()
    {
        canSweepTimes = GetTime(10);
        if(canSweepTimes == 0)
        {
            UI_ShowResource.Instance.UI_Exchange.ShowPanelBuyStamina();
            return;
        }
        
        ClearListSweeps();

        SetButtonsActive(false);

        for (int i = 0; i < canSweepTimes; i++)
        {
            var go = Instantiate(listSweep, panelListSweep);
            var ui = go.GetComponent<UI_ListSweep>();

            ui.SetText(i + 1);
            ui.SetItemDrop(DropSystem.RollDrops());

            go.SetActive(false);
            sweepItems.Add(go);
        }

        showCoroutine = StartCoroutine(ShowSequential());
    }

    IEnumerator ShowSequential()
    {
        isSequentialRunning = true;

        foreach (var item in sweepItems)
        {
            item.SetActive(true);
            ScrollToBottom();
            yield return new WaitForSeconds(0.25f);
        }

        isSequentialRunning = false;
        SetButtonsActive(true);
    }

    void SkipAll()
    {
        if (showCoroutine != null)
            StopCoroutine(showCoroutine);

        foreach (var item in sweepItems)
        {
            item.SetActive(true);
        }

        ScrollToBottom();

        isSequentialRunning = false;

        StartCoroutine(EnableButtonsNextFrame());
    }

    IEnumerator EnableButtonsNextFrame()
    {
        yield return null;
        SetButtonsActive(true);
    }

  

    void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            scrollRect.content.GetComponent<RectTransform>());

        if (scrollCoroutine != null)
            StopCoroutine(scrollCoroutine);

        scrollCoroutine = StartCoroutine(SmoothScrollToBottom());
    }

    IEnumerator SmoothScrollToBottom()
    {
        float duration = 0.25f;
        float time = 0f;

        float start = scrollRect.verticalNormalizedPosition;
        float target = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Ease Out Cubic (mượt hơn)
            t = 1f - Mathf.Pow(1f - t, 3f);

            scrollRect.verticalNormalizedPosition = Mathf.Lerp(start, target, t);

            yield return null;
        }

        scrollRect.verticalNormalizedPosition = target;
    }

    
}
