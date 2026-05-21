using UnityEngine;

public class UI_ListTriviaReward : MonoBehaviour
{
    [SerializeField] private TriviaQuestionType type;

    [SerializeField] private Transform panelReward;
    [SerializeField] private GameObject rewardPrefab;

    private void Start()
    {
        SetUp();
    }
    public void SetUp()
    {    TriviaRewardSO triviaRewardSO = TriviaQuestionState.Instance.GetTriviaRewardSOByType(type);
         if(triviaRewardSO == null)
             return;
        int currentCorrectAnswer = TriviaQuestionState.Instance.GetCorrectAnswerByType(type);
        foreach(TriviaReward trivia in triviaRewardSO.triviaList)
        {
            var rewardObject = Instantiate(rewardPrefab, panelReward);
            rewardObject.SetActive(false);
            UI_TriviaReward triviaReward = rewardObject.GetComponent<UI_TriviaReward>();
            triviaReward.SetUp(trivia, currentCorrectAnswer, type);
            rewardObject.SetActive(true);
        }
    }
}
