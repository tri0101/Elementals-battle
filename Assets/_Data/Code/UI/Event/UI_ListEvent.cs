using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UI_ListEvent : MonoBehaviour
{
    [System.Serializable]
    public class TabData
    {
        public Button button;
        public GameObject panel;
        public Image buttonImage;
    }

    [SerializeField] private List<TabData> tabs;

    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite normalSprite;

    private int currentIndex = -1;

    private void Awake()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            int index = i;

            tabs[i].button.onClick.RemoveAllListeners();
            tabs[i].button.onClick.AddListener(() => SelectTab(index));
        }
    }

    private void Start()
    {
        SelectTab(0);
    }

    private void SelectTab(int index)
    {
       
        if (currentIndex == index)
            return;

        for (int i = 0; i < tabs.Count; i++)
        {
            bool isSelected = (i == index);

            tabs[i].panel.SetActive(isSelected);

            if (tabs[i].buttonImage != null)
            {
                tabs[i].buttonImage.sprite =
                    isSelected ? selectedSprite : normalSprite;
            }
        }

        currentIndex = index;
    }
}