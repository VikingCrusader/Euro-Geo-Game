// Author: Yiwen Zhang ｜ Project: Euro Geo Game
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EuroGeoGame
{
    // Represents a single quiz question
    public class Question
    {
        public string Prompt { get; set; }
        public string Answer { get; set; }
        public string Difficulty { get; set; }
        // Constructor to initialize the question, answer, and difficulty
        public Question(string prompt, string answer, string difficulty)
        {
            Prompt = prompt;
            Answer = answer.Trim(); // Remove extra spaces
            Difficulty = difficulty;
        }

        // Checks if user's input matches the correct answer (case and spaces insensitive)
        public bool Check(string userAnswer) =>
            userAnswer.Trim().ToLower() == Answer.ToLower();
    }


    // Manages loading of question data
    public static class DataManager
    {
        // Loads questions from text files
        public static List<Question> LoadQuestions(string filePath)
        {
            var list = new List<Question>();
            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split(';');
                if (parts.Length >= 3)
                    list.Add(new Question(parts[0], parts[1], parts[2]));
            }
            return list;
        }
    }

    // Controls the gameplay, scoring, and achievements
    public class GameSession
    {
        private readonly List<Question> questions;
        private readonly string difficulty;
        private int correct = 0, wrong = 0;

        private static int totalQuestionsAnswered = 0;
        private static int maxStreak = 0;
        private static double maxAccuracy = 0;

        public GameSession(List<Question> questions, string difficulty)
        {
            this.difficulty = difficulty;
            // Shuffle and filter questions by selected difficulty
            this.questions = questions.Where(q => q.Difficulty == difficulty).OrderBy(_ => Guid.NewGuid()).ToList();
        }

        // Tracks current and total session achievements
        private int streak = 0;
        private static int totalAnswered = 0;
        private static bool streakUnlocked = false;
        private static bool hundredUnlocked = false;

        // Main loop of the game session
        public void Start()
        {
            Console.WriteLine($"\nStarting game in {difficulty} mode. Type 'stop' to end the session at any time.\n");

            int questionIndex = 0;
            var used = new HashSet<int>(); // Avoid repeated questions

            while (used.Count < questions.Count)
            {
                int index;
                // Randomly pick an unused question
                do { index = new Random().Next(questions.Count); } while (used.Contains(index));
                used.Add(index);

                var q = questions[index];
                Console.WriteLine($"Q{used.Count}: {q.Prompt}");
                Console.Write("Your answer (or 'stop'): ");
                var input = Console.ReadLine();

                if (input.Trim().ToLower() == "stop")
                {
                    Console.WriteLine("\nGame stopped by user.");
                    break;
                }

                totalAnswered++;

                if (q.Check(input))
                {
                    correct++;
                    streak++;
                    Console.WriteLine("✅Correct! +1 point\n");

                    // Feedback based on streak length
                    if (streak >= 10 && streak < 20)
                    {
                        Console.WriteLine($"🔥 {streak} correct answers in a row! You're on fire!");
                    }
                    else if (streak >= 20 && streak < 50)
                    {
                        Console.WriteLine($"🔥🔥 RAMPAGE! You're on a {streak}-answer rampage!");
                    }
                    else if (streak >= 50)
                    {
                        Console.WriteLine($"🔥🔥🔥 LEGENDARY! {streak} correct answers in a row! UNSTOPPABLE!");
                    }
                }
                else
                {
                    wrong++;
                    if (streak >= 10)
                        Console.WriteLine($"💧 Streak broken! You were on a {streak} answer streak. Keep going!");
                    streak = 0;
                    Console.WriteLine($"❌Wrong! Correct answer: {q.Answer}\n");
                }

                // Unlock achievement after 100 answered
                if (totalAnswered == 100 && !hundredUnlocked)
                {
                    Console.WriteLine("🌟 Achievement unlocked: 100 questions answered! True Geography Warrior!");
                    hundredUnlocked = true;
                }
            }

            ShowResults();
        }

        // Shows score, accuracy, and achievement summary
        private void ShowResults()
        {
            int total = correct + wrong;
            Console.WriteLine("--- Game Over ---");
            Console.WriteLine($"Answered: {total}, Correct: {correct}, Wrong: {wrong}");

            if (total == 0)
            {
                Console.WriteLine("No questions were answered.");
                return;
            }

            double rate = correct * 100.0 / total;
            Console.WriteLine($"Success rate: {rate:F1}%");

            // Feedback message based on accuracy
            if (rate == 100)
                Console.WriteLine("🏆 Geography Genius! You know Europe better than a GPS!");
            else if (rate >= 90)
                Console.WriteLine("💯 Excellent! You could be a tour guide in all of Europe!");
            else if (rate >= 80)
                Console.WriteLine("👏 Great job! That brain of yours has seen some maps!");
            else if (rate >= 70)
                Console.WriteLine("🎒 Nice work! A bit more and you’ll be ready for a geography bee.");
            else if (rate >= 60)
                Console.WriteLine("📘 Decent effort! Your atlas would be proud.");
            else if (rate >= 50)
                Console.WriteLine("🙂 Not bad! But you might still get lost in Brussels...");
            else if (rate >= 40)
                Console.WriteLine("🧭 Hmm... at least you know Europe is not in Asia.");
            else if (rate >= 30)
                Console.WriteLine("🌍 Well... you know 'some places' exist. Keep going!");
            else if (rate >= 20)
                Console.WriteLine("😅 You tried! That’s worth something, right?");
            else if (rate >= 10)
                Console.WriteLine("📉 Geography may not be your thing. But hey, there’s always Google Maps!");
            else
                Console.WriteLine("❌ 0%?! Have you ever seen a map before? 😂 Shame on you!");

            // Update global stats
            totalQuestionsAnswered += correct + wrong;

            if (streak > maxStreak)
                maxStreak = streak;

            if (rate > maxAccuracy)
                maxAccuracy = rate;

            Console.WriteLine($"\n📊 Total questions answered so far: {totalQuestionsAnswered}");
            Console.WriteLine($"🏅 Highest streak ever: {maxStreak}");
            Console.WriteLine($"📈 Highest accuracy ever: {maxAccuracy:F2}%");
        }
    }

    // Program entry point and user interface menu
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🌍 Welcome to the Euro Geo Game! 🌍");
            Console.WriteLine("🌍 Test your knowledge of European countries, capitals, cities, features, neighbors and area sizes! 🌍");
            Console.WriteLine("🌍 The capitalization and spaces don't matter at all — just have fun learning! 🌍");
            Console.WriteLine("🌍 You can quit anytime by typing 'stop'. 🌍");
            Console.WriteLine("🌍 Accuracy matters — you’ll see your score, accurate rate and feedback at the end of each game. 🌍");
            Console.WriteLine("✅Choose your difficulty level to begin: Easy / Medium / Hard✅\n");

            string difficulty = Console.ReadLine().Trim().ToLower();
            if (difficulty == "easy") difficulty = "Easy";
            else if (difficulty == "medium") difficulty = "Medium";
            else if (difficulty == "hard") difficulty = "Hard";
            else
            {
                Console.WriteLine("Invalid input, defaulting to Medium.");
                difficulty = "Medium";
            }

            while (true)
            {
                // Game mode menu
                Console.WriteLine("\n🎮 Select a game mode:\n");
                Console.WriteLine("  1. Country ➔ Capital");
                Console.WriteLine("     I’ll name a European country — you type its capital city.\n");
                Console.WriteLine("  2. City ➔ Country");
                Console.WriteLine("     I’ll give you a European city (not a capital) — you say which country it belongs to.\n");
                Console.WriteLine("  3. Feature ➔ Country");
                Console.WriteLine("     I’ll describe a European country with a fun fact or historical clue — you guess the country.\n");
                Console.WriteLine("  4. Largest Neighbor ➔ Country");
                Console.WriteLine("     I’ll name a country — you tell me its largest neighboring country.\n");
                Console.WriteLine("  5. Random Mixed Challenge");
                Console.WriteLine("     A mix of 1 and 2. If I give you a city, you type which country it belongs to; If I give you a country, you give me its capital city. They will appear randomly.\n");
                Console.WriteLine("  6. Change Difficulty");
                Console.WriteLine("     Switch between Easy / Medium / Hard mode at any time.\n");
                Console.WriteLine("  7. Exit Game");
                Console.WriteLine("     Leave the game — see you next time!\n");

                string key = Console.ReadLine();
                if (key == "7") break;

                List<Question> questions = new();

                switch (key)
                {
                    case "1":
                        questions = DataManager.LoadQuestions("data/capitals.txt");
                        break;
                    case "2":
                        questions = DataManager.LoadQuestions("data/cities.txt");
                        break;
                    case "3":
                        questions = DataManager.LoadQuestions("data/features.txt");
                        break;
                    case "4":
                        questions = DataManager.LoadQuestions("data/largest_neighbors.txt");
                        break;
                    case "5":
                        try
                        {
                            // Combine capitals and cities into a random mix
                            var mixedQuestions = new List<Question>();
                            var sources = new[]
                            {
                                "data/capitals.txt",
                                "data/cities.txt",
                            };

                            foreach (var file in sources)
                            {
                                mixedQuestions.AddRange(
                                    DataManager.LoadQuestions(file)
                                    .Where(q => q.Difficulty == difficulty)
                                );
                            }

                            mixedQuestions = mixedQuestions.OrderBy(_ => Guid.NewGuid()).ToList();
                            var mixedSession = new GameSession(mixedQuestions, difficulty);
                            mixedSession.Start();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("❌ Error loading questions: " + ex.Message);
                        }
                        continue;
                    case "6":
                        Console.WriteLine("\n🔧 Change difficulty level:");
                        Console.WriteLine("Enter new difficulty (Easy / Medium / Hard):");
                        string newDifficulty = Console.ReadLine().Trim().ToLower();
                        if (newDifficulty == "easy") newDifficulty = "Easy";
                        else if (newDifficulty == "medium") newDifficulty = "Medium";
                        else if (newDifficulty == "hard") newDifficulty = "Hard";
                        else
                        {
                            Console.WriteLine("❌ Invalid difficulty. No change made.");
                            continue;
                        }

                        difficulty = newDifficulty;
                        Console.WriteLine($"✅ Difficulty updated to {difficulty}.");
                        continue;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        continue;
                }

                var session = new GameSession(questions, difficulty);
                session.Start();
            }

            Console.WriteLine("\nThanks for playing the Euro Geo Game! 👋");
        }
    }
}
