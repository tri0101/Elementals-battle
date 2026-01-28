using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_FormationSlot : MonoBehaviour
{
    [Header("Slot config")]
    public int slotIndex = 1;

    [Header("UI")]
    public Button buttonSlot;
    public TextMeshProUGUI nameText;
    public Transform previewRoot;
    public Transform starRoot; 

    HeroViewData current = null;
    GameObject currentPreview = null;

    void Awake()
    {
        if (buttonSlot != null)
        {
            buttonSlot.onClick.RemoveAllListeners();
            buttonSlot.onClick.AddListener(OnClickSlot);
        }
    }

    void OnDestroy()
    {
        if (buttonSlot != null)
            buttonSlot.onClick.RemoveAllListeners();
    }

    void OnClickSlot()
    {
       
        FormationContext.SelectedSlotIndex = slotIndex;
        FormationContext.SelectedHero = null;

        GameManager.Instance.LoadAdditiveScene(SceneId.HeroSelectScene);
    }

    public void SetupSlot(int heroId, HeroDatabase heroDatabase)
    {
        if (currentPreview != null)
        {
            Destroy(currentPreview);
            currentPreview = null;
        }

        current = null;

        //if (heroId <= 0)
        //{
            
        //    if (icon != null) icon.sprite = null;
        //    if (nameText != null) nameText.text = "";
        //    UpdateStarVisual(0);
        //    return;
        //}

        
        HeroViewData hv = null;
        if (PlayerInventory.Instance != null && heroDatabase != null)
        {
            var list = PlayerInventory.Instance.GetHeroViewList(heroDatabase);
            hv = list.Find(x => x != null && x.instance != null && x.instance.heroId == heroId);
        }

        HeroInfo info = heroDatabase != null ? heroDatabase.GetHero(heroId) : null;

        if (hv != null)
        {
            current = hv;
            if (nameText != null) nameText.text = hv.info.Name;
            UpdateStarVisual(hv.instance.star);
            SpawnPreview(hv.info);
        }
        else if (info != null)
        {
            if (nameText != null) nameText.text = info.Name;
            UpdateStarVisual(0);
            SpawnPreview(info);
        }
        else
        {

            if (nameText != null) nameText.text = "";
            UpdateStarVisual(0);
        }
    }

    void UpdateStarVisual(int starCount)
    {
        if (starRoot == null) return;
        for (int i = 0; i < starRoot.childCount; i++)
            starRoot.GetChild(i).gameObject.SetActive(i < starCount);
    }

    void SpawnPreview(HeroInfo info)
    {
        if (info == null || info.HeroPreviewPrefabs == null || previewRoot == null) return;

        currentPreview = GameObject.Instantiate(info.HeroPreviewPrefabs, previewRoot);
        //currentPreview.transform.localPosition = Vector3.zero;
        //currentPreview.transform.localRotation = Quaternion.identity;
        //currentPreview.transform.localScale = Vector3.one;
    }
}