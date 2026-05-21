using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class TriviaQuestion
{
    public int questionID;
    public string questionText;
    public string[] options;
    public int correctOptionIndex; 
}
[CreateAssetMenu(menuName = "Event/TriviaQuestionSO")]
public class TriviaQuestionSO : ScriptableObject
{

    public List<TriviaQuestion> triviaList = new List<TriviaQuestion>();

    public TriviaQuestion GetTriviaQuestion(int questionID)
    {
        TriviaQuestion trivia = triviaList.Find(x => x.questionID == questionID);
        if (trivia != null) return trivia;
        else return null;
    }
}
