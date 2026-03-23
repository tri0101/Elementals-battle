using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_ListStoreItem : MonoBehaviour
{
    [SerializeField] private GameObject storeItemPrefabs;
    [SerializeField] private Transform contentItems;
    [SerializeField] private Button buttonRefresh;
    [SerializeField] private Button buttonOk;
    [SerializeField] private Button buttonclose;
    [SerializeField] private Transform panelRefresh;
    [Header("Effect Settings")]
    [SerializeField] private float rotateDuration = 1f;
    [SerializeField] private float flashSpeed = 8f;
    [SerializeField] private UI_PanelStoreDetail detailStore;   
    

    private bool isRefreshing = false;

    void OnEnable()
    {
        
        BuildShop();

        buttonRefresh.onClick.RemoveAllListeners();
        buttonRefresh.onClick.AddListener(() =>
        {
            CheckRefresh();
            panelRefresh.gameObject.SetActive(true);
            
        });
        buttonclose.onClick.RemoveAllListeners();
        buttonclose.onClick.AddListener(()=>ClosePanelRefresh());
        buttonOk.onClick.RemoveAllListeners();
        buttonOk.onClick.AddListener(() =>
        {
            if (!isRefreshing)
                StartCoroutine(RefreshShop());
        });
        if (TimeManager.Instance.ShouldResetShop())
        {
            StartCoroutine(RefreshShop());
        }
    }
  

    void CheckRefresh()
    {
        if(PlayerInventory.Instance.GetItemQuantity(2) < 50)
        {
            buttonOk.interactable = false;
            return;
        }
        else
        {
            buttonOk.interactable = true;
        }
           
       
    }
    IEnumerator RefreshShop()
        
    {
        detailStore.gameObject.SetActive(false);
        PlayerInventory.Instance.ConsumeItem(2, 50);
        panelRefresh.gameObject.SetActive(false);
        isRefreshing = true;
        buttonRefresh.interactable = false;

        float duration = rotateDuration;
        float timer = 0f;

        int childCount = contentItems.childCount;

        Quaternion[] startRot = new Quaternion[childCount];
        UI_StoreItem[] items = new UI_StoreItem[childCount];

        for (int i = 0; i < childCount; i++)
        {
            Transform t = contentItems.GetChild(i);
            startRot[i] = t.rotation;

            items[i] = t.GetComponent<UI_StoreItem>();

            if (items[i] != null)
                StartCoroutine(items[i].PlayGlow(duration));
        }

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            for (int i = 0; i < childCount; i++)
            {
                Transform item = contentItems.GetChild(i);
                item.rotation = startRot[i] * Quaternion.Euler(0, 360f * t, 0);
            }

            yield return null;
        }

        // reset rotation
        for (int i = 0; i < childCount; i++)
        {
            Transform item = contentItems.GetChild(i);
            item.rotation = startRot[i];
        }

        ClearOldItems();
        BuildShop();

        buttonRefresh.interactable = true;
        isRefreshing = false;
    }
    void BuildShop()
    {
        SetUpItem(100);
        GetNeedItems();
        GetPreviousNeedItems();
        SetUpFood();
        SetUpShard();
        SetUpItem(4);
    }
    
    void ClosePanelRefresh()
    {
        panelRefresh.gameObject.SetActive(false);
    }
    void SetUpFood()
    {
        SetUpItem(UnityEngine.Random.Range(50, 53));
    }

    void SetUpShard()
    {
        SetUpItem(UnityEngine.Random.Range(1001, 1004));
    }

    void GetPreviousNeedItems()
    {
        int chapter = ProgressManager.Instance.GetChapter();

        if (chapter == 1 || chapter == 2)
        {
            SetUpItem(101);
            SetUpItem(102);
            SetUpItem(103);
        }
    }

    void GetNeedItems()
    {
        int chapter = ProgressManager.Instance.GetChapter();

        if (chapter == 1)
        {
            SetUpItem(101);
            SetUpItem(102);
            SetUpItem(103);
        }
        else if (chapter == 2)
        {
            SetUpItem(104);
            SetUpItem(105);
            SetUpItem(106);
        }
    }

    public void SetUpItem(int itemId)
    {
        ShopItemData shopItemData =
            DatabaseManager.Instance.ShopItemDatabase.GetShopItemData(itemId);

        ItemData itemData =
            DatabaseManager.Instance.ItemDatabase.GetItem(itemId);

        var go = Instantiate(storeItemPrefabs, contentItems);
        var ui = go.GetComponent<UI_StoreItem>();

        if (ui != null)
            ui.SetUp(itemData, shopItemData);
    }

    void ClearOldItems()
    {
        foreach (Transform child in contentItems)
            Destroy(child.gameObject);
    }
    private void Update()
    {
        if(TimeManager.Instance.ShouldResetShop())
        {
            StartCoroutine(RefreshShop());
        }
    }


}