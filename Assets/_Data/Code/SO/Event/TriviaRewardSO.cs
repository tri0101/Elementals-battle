using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TriviaReward
{
    public int rewardID;
    public List<ItemAndAmount> rewardItems = new List<ItemAndAmount>();
    public int questionsToClaim; // số câu hỏi trả lơi để nhận phần thưởng
}
[CreateAssetMenu(menuName = "Event/TriviaRewardSO")]
public class TriviaRewardSO : ScriptableObject
{
    public TriviaQuestionType triviaType;
    public List<TriviaReward> triviaList = new List<TriviaReward>();

    public TriviaReward GetTriviaReward(int rewardID)
    {
        TriviaReward triviaReward = triviaList.Find(x => x.rewardID == rewardID);
        if (triviaReward != null) return triviaReward;
        else return null;
    }
}
