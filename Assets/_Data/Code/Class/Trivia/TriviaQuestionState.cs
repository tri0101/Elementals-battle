using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
public enum TriviaQuestionType
{
   Trivia,
   WorldCup2026
}
[System.Serializable]
public class TriviaQuestionStateData
{
    public int questionID;
    public bool isCorrect; //trả lời đúng hay sai
}
[System.Serializable]
public class TriviaQuestionRewardStateData
{
    public int rewardID;
    public bool isCompleted; // đã đạt chưa
    public bool isClaimed; // đã nhận chưa
}
[System.Serializable]
public class TriviaQuestionProgress
{
    public TriviaQuestionType questionType;
    public List<TriviaQuestionStateData> questionStates = new List<TriviaQuestionStateData>();
    public List<TriviaQuestionRewardStateData> rewardStates = new List<TriviaQuestionRewardStateData>();
    public int currentQuestion; // đang trả lời đến câu nào
    public int correctAnswers; // số câu trả lời đúng

}
public class TriviaQuestionState : Subject
{
    public static TriviaQuestionState Instance { get; private set; }

    public List<TriviaQuestionProgress> triviaProgressList = new List<TriviaQuestionProgress>();
    public List<TriviaRewardSO> triviaRewardSOList = new List<TriviaRewardSO>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        Init();
    }
    public TriviaRewardSO GetTriviaRewardSOByType(TriviaQuestionType type)
    {
        TriviaRewardSO triviaRewardSO = 
            triviaRewardSOList.Find(x => x.triviaType == type);
        if (triviaRewardSO != null) return triviaRewardSO;
            return null;
    }
    private void Init()
    {

        foreach (TriviaQuestionType type in System.Enum.GetValues(typeof(TriviaQuestionType)))
        {
            var progress = triviaProgressList.Find(p => p.questionType == type);
            if (progress == null)
            {
                progress = new TriviaQuestionProgress
                {
                    questionType = type,
                    currentQuestion = 1,
                    correctAnswers = 0
                };
                triviaProgressList.Add(progress);
            }

            EnsureRewardStatesForType(progress);
        }
    }

    private void EnsureRewardStatesForType(TriviaQuestionProgress progress)
    {
        if (progress == null)
            return;

        if (triviaRewardSOList == null || triviaRewardSOList.Count == 0)
            return;

        
        for (int soIndex = 0; soIndex < triviaRewardSOList.Count; soIndex++)
        {
            TriviaRewardSO so = triviaRewardSOList[soIndex];
            if (so == null) continue;
            if (so.triviaType != progress.questionType) continue;
            if (so.triviaList == null) continue;

            
            for (int i = 0; i < so.triviaList.Count; i++)
            {
                var reward = so.triviaList[i];
                if (reward == null) continue;

                int rewardId = reward.rewardID;

                if (progress.rewardStates.Exists(x => x.rewardID == rewardId))
                    continue;

                progress.rewardStates.Add(new TriviaQuestionRewardStateData
                {
                    rewardID = rewardId,
                    isCompleted = false,
                    isClaimed = false
                });
            }
        }
    }
    public int GetCorrectAnswerByType(TriviaQuestionType type)
    {
        TriviaQuestionProgress progress = triviaProgressList.Find(
            progress => progress.questionType == type
        );

        return progress != null ? progress.correctAnswers : 0;
    }
    public int GetCurrentQuestionByType(TriviaQuestionType type)
    {
        TriviaQuestionProgress progress = triviaProgressList.Find(
            progress => progress.questionType == type
        );

        return progress != null ? progress.currentQuestion : 0;
    }
    public bool GetIsClamied(TriviaQuestionType type, int rewardId)
    {
        var progress = triviaProgressList.Find(p => p.questionType == type);
        if (progress == null)
            return false;

        EnsureRewardStatesForType(progress);

        var state = progress.rewardStates.Find(r => r.rewardID == rewardId);
        return state != null && state.isClaimed;
    }
    public void AddCurrentQuestionByType(TriviaQuestionType type)
    {
        triviaProgressList.Find(
            progress => progress.questionType == type).currentQuestion++;

        NotifyObservers();
    }
    public void AddCorrectAnswerByType(TriviaQuestionType type)
    {
        triviaProgressList.Find(
            progress => progress.questionType == type).correctAnswers++;
        NotifyObservers();
    }
    public void SetProgress(TriviaQuestionType type, int questionID, bool isCorrect)
    {
        var progress = triviaProgressList.Find(p => p.questionType == type);
        if (progress == null)
        {
            progress = new TriviaQuestionProgress { questionType = type };
            triviaProgressList.Add(progress);
        }
        progress.questionStates.Add(new TriviaQuestionStateData
        {
            questionID = questionID,
            isCorrect = isCorrect
        });
        
        AddCurrentQuestionByType(type);
        if(isCorrect)
            AddCorrectAnswerByType(type);
        NotifyObservers();
        UpdateProgress(type);
        NotifyObservers(type);
    }
    void UpdateProgress(TriviaQuestionType type)
    {
        var progress = triviaProgressList.Find(p => p.questionType == type);
        if (progress == null)
            return;

    
        EnsureRewardStatesForType(progress);

        int correctAnswer = progress.correctAnswers;

        
        for (int soIndex = 0; soIndex < triviaRewardSOList.Count; soIndex++)
        {
            TriviaRewardSO so = triviaRewardSOList[soIndex];
            if (so == null) continue;
            if (so.triviaType != type) continue;
            if (so.triviaList == null) continue;

            for (int i = 0; i < so.triviaList.Count; i++)
            {
                var rewardConfig = so.triviaList[i];
                if (rewardConfig == null) continue;

                int rewardId = rewardConfig.rewardID;

                var state = progress.rewardStates.Find(x => x.rewardID == rewardId);
                if (state == null)
                {
                    state = new TriviaQuestionRewardStateData
                    {
                        rewardID = rewardId,
                        isCompleted = false,
                        isClaimed = false
                    };
                    progress.rewardStates.Add(state);
                }

                if (!state.isCompleted && correctAnswer >= rewardConfig.questionsToClaim)
                {
                    state.isCompleted = true;
                    NotifyObservers(type);
                }
            }
        }
        NotifyObservers();
    }

    public void SetRewardProgress(TriviaQuestionType type, int rewardID, bool isCompleted, bool isClaimed)
    {
        var progress = triviaProgressList.Find(p => p.questionType == type);
        if (progress == null)
        {
            progress = new TriviaQuestionProgress
            {
                questionType = type,
                currentQuestion = 1,
                correctAnswers = 0
            };
            triviaProgressList.Add(progress);

            EnsureRewardStatesForType(progress);
        }

        var existing = progress.rewardStates.Find(x => x.rewardID == rewardID);
        if (existing != null)
        {
            existing.isCompleted = isCompleted;
            existing.isClaimed = isClaimed;
            return;
        }

        progress.rewardStates.Add(new TriviaQuestionRewardStateData
        {
            rewardID = rewardID,
            isCompleted = isCompleted,
            isClaimed = isClaimed
        });
        NotifyObservers();
    }
    public List<TriviaQuestionProgress> GetListTriviaQuestion()
    {
        return triviaProgressList;
    }
    public void SetListTriviaQuestion(List<TriviaQuestionProgress> list)
    {
        this.triviaProgressList = list;
    }
}
