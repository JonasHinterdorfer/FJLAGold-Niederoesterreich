using System.Security.Cryptography;

namespace FJLAGoldFragen;

class Program
{
    public static void Main(string[] args)
    {
        Console.CursorVisible = false;
        var lines = ReadCSV("questions.csv");
        var splitLines = SplitLines(lines, ";");
        var questions = EncapsulateQuestions(splitLines);
        bool isTestSim = UseTestSimulation();

        if (isTestSim)
        {
            questions = RemoveRandomItems(questions, 20);
        }
        
        WriteQuestions(questions, isTestSim);
        Console.ReadKey();
    }

    private static Question[] RemoveRandomItems(Question[] questions, int amount)
    {
        List < Question > questionsList = questions.ToList();

        for (int i = 0; i < amount; i++)
        {
            Random rnd = new Random();
            int index = rnd.Next(0, questionsList.Count);
            questionsList.RemoveAt(index);
        }

        return questionsList.ToArray();
    }

    private static string[] ReadCSV(string fileName)
    {
        return File.ReadAllLines(fileName);
    }

    private static List<string[]> SplitLines(string[] lines, string separator)
    {
        List<string[]> splitLines = new List<string[]>();

        foreach (string line in lines)
        {
            splitLines.Add(line.Split(separator));
        }

        return splitLines;
    }

    private static Question[] EncapsulateQuestions(List<string[]> splitLines)
    {
        RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
        Question[] questions = new Question[splitLines.Count];

        for (int questionNumber = 0; questionNumber < splitLines.Count; questionNumber++)
        {
            questions[questionNumber] = new Question();

            string[] questionArray = splitLines[questionNumber];
            questions[questionNumber].QuestionSentence = questionArray[0];
            questions[questionNumber].RightAnswer = questionArray[1];
            questionArray= questionArray.Where(w => w != questionArray[0]).ToArray(); //
            questions[questionNumber].Answers = questionArray;
            
            questions[questionNumber].Answers = questions[questionNumber].Answers.OrderBy(x =>  GetNextInt32(random)).ToArray();
        }
        
        return questions;
    }

    private static void WriteQuestions(Question[] questions, bool isTestSim)
    {
        for (int i = 0; i < questions.Length; i++)
        {
            var question = questions[i];
            WriteQuestion(question);
            WriteSpaceLines(2);
            var answer = GetAnswer("Deine Antwort: ");
            
            if (answer > 0)
            {
                bool isCorrect = CalcualteCorrectness(question, answer);
                question.AnswerdCorrect = isCorrect;
                
                if(!isTestSim)
                    ReviewCorrectness(isCorrect, question.RightAnswer);
                
            }
            else
            {
                WriteColored("Gib etwas Richtiges ein!", ConsoleColor.DarkRed);
                i--;
                
            }
            if(!isTestSim)
                Console.ReadKey();
            Console.Clear();
            
            questions[i] = question;
        }
        ReviewStats(questions);
    }

    private static bool CalcualteCorrectness(Question question, int answer)
    {
        return question.RightAnswerIndex == answer - 1;
    }

    private static void WriteQuestion(Question question)
    {
        WriteColored(question.QuestionSentence, ConsoleColor.DarkGreen);
        Console.WriteLine();
        for (int i = 0; i < question.Answers.Length; i++)
        {
            var answer = question.Answers[i];
            Console.WriteLine($"{i+1}) {answer}");
        }
    }
    

    private static void WriteColored(Object toWrite, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(toWrite.ToString());
        Console.ForegroundColor = ConsoleColor.White;
    }
    
    private static void ReviewCorrectness(bool isCorrect, string correctAnswer)
    {
        WriteSpaceLines(2);
        if (isCorrect)
        {
            WriteColored("Richtig", ConsoleColor.Green);
        }
        else
        {
            WriteColored("Falsch", ConsoleColor.Red);
            Console.WriteLine($"Richtig wäre: \n{correctAnswer}");
        }
    }
    
    private static int GetNextInt32(RNGCryptoServiceProvider rnd)
    {
        byte[] randomInt = new byte[4];
        rnd.GetBytes(randomInt);
        return Convert.ToInt32(randomInt[0]);
    }

    private static int GetAnswer(string answerPromt)
    {
        Console.Write(answerPromt);

        if (int.TryParse(Console.ReadLine()!, out int answer))
        {
            return answer;
        }
        
        return -1;
    }

    private static void WriteSpaceLines(int amount)
    {
        for (int i = 0; i <= amount; i++)
        {
            Console.WriteLine();
        }
    }

    private static void ReviewStats(Question[] questions)
    {
        int amountOfCorrectAnswerdQuestions = 0;

        foreach (Question question in questions)
        {

            if (question.AnswerdCorrect)
            {
                amountOfCorrectAnswerdQuestions++;
            }
        }
        
        Console.WriteLine($"Du hast {amountOfCorrectAnswerdQuestions} von {questions.Length} richtig!");
    }

    private static bool UseTestSimulation()
    {
        while (true)
        {
            Console.Write("Möchtest du einen Test simulieren? (j/n): ");
            string answer = Console.ReadLine();

            switch (answer.ToLower())
            {
                case "j":
                    Console.Clear();
                    return true;
                case "n":
                    Console.Clear();
                    return false;
            }
            Console.WriteLine("Bitte gib j oder n ein!");
        }
    }
    
}