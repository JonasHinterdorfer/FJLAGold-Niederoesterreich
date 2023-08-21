namespace FJLAGoldFragen;

public class Question
{
    public string QuestionSentence { get; set; }
    public string[] Answers { get; set; }
    
    public string RightAnswer { get; set; }

    public int RightAnswerIndex => Array.IndexOf(Answers, RightAnswer);
    public bool AnswerdCorrect { get; set; }
    
}