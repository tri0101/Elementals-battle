using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum GachaCostType
{
    Diamond,
    Ticket,
    Null
}
public enum GachaCostTypeID
{
    Diamond = 2,
    CommonTicket = 4,
    PremiumTicket = 7,
    EventTicket = 12
}
[System.Serializable]
public struct GachaCost
{
    public int rollOne;
    public int rollTen;
}

public class UI_CostGacha : MonoBehaviour
{
    [Header("Icon")]
    [SerializeField] private Sprite diamondSprite;
    [SerializeField] private Sprite ticketSprite;

    [Header("Roll 1 UI")]
    [SerializeField] private Image oneRollIcon;
    [SerializeField] private TextMeshProUGUI oneRollCostText;
    [SerializeField] private Button oneRollButton;

    [Header("Roll 10 UI")]
    [SerializeField] private Image tenRollIcon;
    [SerializeField] private TextMeshProUGUI tenRollCostText;
    [SerializeField] private Button tenRollButton;

    [Header("Cost Config")]
    //[SerializeField] private int diamondCostOne = 280;
    //public int DiamondCostOne => diamondCostOne;
    //[SerializeField] private int diamondCostTen = 2588;
    //public int DiamondCostTen => diamondCostTen;
    //[SerializeField] private int ticketCostOne = 1;
    //public int TicketCostOne => ticketCostOne;
    //[SerializeField] private int ticketCostTen = 10;
    //public int TicketCostTen => ticketCostTen;

    [SerializeField] private GachaCostTypeID diamondID;
    public GachaCostTypeID DiamondID => diamondID;
    [SerializeField] private GachaCostTypeID ticketID;
    public GachaCostTypeID TicketID => ticketID;
    [SerializeField] private GachaCost diamondCost;
    public GachaCost DiamondCost => diamondCost;
    [SerializeField] private GachaCost  ticketCost;
    public GachaCost TicketCost => ticketCost;
    [SerializeField] private GachaCostType currentTypeOne;
    public GachaCostType CurrentTypeOne => currentTypeOne;
    [SerializeField]  private GachaCostType currentTypeTen;
    public GachaCostType CurrentTypeTen => currentTypeTen;

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        SetupRollOne();
        SetupRollTen();
    }

    private void Awake()
    {
        RefreshUI();
    }

    void SetupRollOne()
    {
        int ticket = GetItem((int)ticketID);
        int diamond = GetItem((int)diamondID);

        if (ticket >= ticketCost.rollOne)
        {
            currentTypeOne = GachaCostType.Ticket;
            SetTicketUI(oneRollIcon, oneRollCostText, oneRollButton,
                        ticket, ticketCost.rollOne);
        }
        else if (diamond >= diamondCost.rollOne)
        {
            currentTypeOne = GachaCostType.Diamond;
            SetDiamondUI(oneRollIcon, oneRollCostText, oneRollButton,
                         diamond, diamondCost.rollOne);
        }
        else
        {
            currentTypeOne = GachaCostType.Diamond;
            SetDiamondUI(oneRollIcon, oneRollCostText, oneRollButton,
                         diamond, diamondCost.rollOne);
        }
    }



    void SetupRollTen()
    {
        int ticket = GetItem((int)ticketID);
        int diamond = GetItem((int)diamondID);

        if (ticket >= ticketCost.rollTen)
        {
            currentTypeTen = GachaCostType.Ticket;
            SetTicketUI(tenRollIcon, tenRollCostText, tenRollButton,
                        ticket, ticketCost.rollTen);
        }
        else if (diamond >= diamondCost.rollTen)
        {
            currentTypeTen = GachaCostType.Diamond;
            SetDiamondUI(tenRollIcon, tenRollCostText, tenRollButton,
                         diamond, diamondCost.rollTen);
        }
        else
        {
            currentTypeTen = GachaCostType.Diamond;
            SetDiamondUI(tenRollIcon, tenRollCostText, tenRollButton,
                         diamond, diamondCost.rollTen);
        }
    }

    int GetItem(int id)
    {
        return PlayerInventory.Instance.GetItemQuantity(id);
    }

    void SetTicketUI(Image icon, TextMeshProUGUI costText, Button btn,
                  int playerTicket, int required)
    {
        icon.sprite = ticketSprite;

        costText.text = playerTicket + "/" + required;
        costText.color = playerTicket >= required ? Color.white : Color.red;

        btn.interactable = playerTicket >= required;
    }
    void SetDiamondUI(Image icon, TextMeshProUGUI costText, Button btn,
                  int playerDiamond, int required)
    {
        icon.sprite = diamondSprite;
        RectTransform rect = icon.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(60, 60);

        costText.text = required.ToString();
        costText.color = playerDiamond >= required ? Color.white : Color.red;

        btn.interactable = playerDiamond >= required;
    }



}