using UnityEngine;
using UnityEngine.UI;

public class UI_HeroFeatured : MonoBehaviour
{
    [SerializeField] private Image heroIcon;
    [SerializeField] private Button selectButton;
    [SerializeField] private Transform blackBack;
    [SerializeField] private Transform tick;

    private int heroId;
    private System.Func<int> getSelectedHeroId;
    private System.Action<int> onClicked;

    public void SetUp(int heroId, System.Func<int> getSelectedHeroId, System.Action<int> onClicked)
    {
        HeroInfo heroInfo = DatabaseManager.Instance.HeroDatabase.GetHero(heroId);
        heroIcon.sprite = heroInfo.iconFace;

        this.heroId = heroId;
        this.getSelectedHeroId = getSelectedHeroId;
        this.onClicked = onClicked;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(HandleClick);

        RefreshVisual();
    }

    void HandleClick()
    {
        onClicked?.Invoke(heroId);
    }

    public void RefreshVisual()
    {
        int selected = getSelectedHeroId != null ? getSelectedHeroId.Invoke() : -1;
        bool isSelected = (selected == heroId);

        if (blackBack != null) blackBack.gameObject.SetActive(isSelected);
        if (tick != null) tick.gameObject.SetActive(isSelected);
    }
}