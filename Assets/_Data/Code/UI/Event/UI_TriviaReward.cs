using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_TriviaReward :  MonoBehaviour, IObserver
{
    [SerializeField] private Button buttonClaim;
    [SerializeField] private TextMeshProUGUI textClaimMax;
    [SerializeField] private GameObject itemReward;
    [SerializeField] private Transform itemRewardParent;
    int needCorrectAnswer;
    TriviaQuestionType currentType;
    TriviaReward triviaReward;
    private void Start()
    {
        if(TriviaQuestionState.Instance != null)
        {
            TriviaQuestionState.Instance.AddObserver(this);
        }
        buttonClaim.onClick.RemoveAllListeners();
        buttonClaim.onClick.AddListener(OnClickClaim);
    }
    void OnClickClaim()
    { 
        UI_CanvasReward.Instance.ClearOldItems();
        foreach (ItemAndAmount itemAndAmount in triviaReward.rewardItems)
        {
            ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(itemAndAmount.itemId);
            UI_CanvasReward.Instance.SetUp(itemData, itemAndAmount.amount);
        }
        UI_CanvasReward.Instance.gameObject.SetActive(true);
        UI_CanvasReward.Instance.ShowReward();
        buttonClaim.interactable = false;
        buttonClaim.GetComponentInChildren<TextMeshProUGUI>().text = "Claimed";
        TriviaQuestionState.Instance.SetRewardProgress(currentType, triviaReward.rewardID,
            true, true);
    }
    public void SetUp(TriviaReward triviaReward, int currentCorrectAnswer, TriviaQuestionType type)
    {
       this.currentType = type;
        this.triviaReward = triviaReward;
        foreach (ItemAndAmount itemAndAmount in triviaReward.rewardItems)
        {
            ItemData itemData = DatabaseManager.Instance.ItemDatabase.GetItem(itemAndAmount.itemId);
            if (itemData != null)
            {
                var itemRewardUI = Instantiate(itemReward, itemRewardParent);
                UI_DropRewardItem drop = itemRewardUI.GetComponent<UI_DropRewardItem>();
                drop.Setup(itemData, itemAndAmount.amount);
                int needCorrectAnswer = triviaReward.questionsToClaim;
                this.needCorrectAnswer = needCorrectAnswer;
                textClaimMax.text = $"({currentCorrectAnswer}/{needCorrectAnswer})";
                bool isClaimed = TriviaQuestionState.Instance.GetIsClamied(type, triviaReward.rewardID);
                bool canClaim = currentCorrectAnswer >= needCorrectAnswer;
                if (canClaim )
                {
                    if (isClaimed)
                    {
                        buttonClaim.interactable = false;
                        buttonClaim.GetComponentInChildren<TextMeshProUGUI>().text = "Claimed";
                    }
                        
                    else
                        buttonClaim.interactable = true;
                }
                else
                {
                    buttonClaim.interactable = false;
                    
                }
            }
        }
        
    }
    public void OnNotify(object data)
    {
        if(data is TriviaQuestionType questionType)
        {
            if(questionType == currentType)
            {
                UpdateCurrentText();
            }
        }
        
    }
    void UpdateCurrentText()
    {
        int currentCorrectAnswer = TriviaQuestionState.Instance.GetCorrectAnswerByType(currentType);
        textClaimMax.text = $"({currentCorrectAnswer}/{needCorrectAnswer})";
        if(TriviaQuestionState.Instance.GetIsClamied(currentType, triviaReward.rewardID)){
            return;
        }
        if(currentCorrectAnswer >= needCorrectAnswer)
        {
            buttonClaim.interactable = true;
        }
        else
        {
            buttonClaim.interactable = false;
        }
    }

    

}
