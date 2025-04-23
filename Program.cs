// Name: Yiwen Zhang ｜ Project: Euro Geo Game
using System;
using System.Collections.Generic;  // using arrays
using System.IO;  // reading text files
using System.Linq;  // using sort algorithms
namespace EuroGeoGame  //including all classes
// Define Classes
{
    //  Questions (The answer is a string)
    public class Question
    {
        public string Prompt { get; set; }  // The property to store the question's prompt (the question itself)
        public string Answer { get; set; }  // To property to the correct answer to the question
        public string Difficulty { get; set; }  // The property of the difficulty level (e.g., Easy, Medium, Hard)

        // Constructor to initialize a Question object with its prompt, answer, and difficulty
        public Question(string prompt, string answer, string difficulty)
        {
            Prompt = prompt;  // Assign the prompt to Prompt property
            Answer = answer.Trim();  // Triming the spaces in case of user entering any spaces
            Difficulty = difficulty;  // Assign the difficulty to Difficulty property
        }

        // Method to check whether the user's answer matches the correct one
        public bool Check(string userAnswer) =>
            userAnswer.Trim().ToLower() == Answer.ToLower();  //Case and spaces insensitive
    }

    public static class DataManager
    {
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
            this.questions = questions.Where(q => q.Difficulty == difficulty).OrderBy(_ => Guid.NewGuid()).ToList(); // 不再截断为10题

        }

        // Achievement System
        private int streak = 0;                     // Tracks correct answers in a row
        private static int totalAnswered = 0;       // Tracks total questions answered
        private static bool streakUnlocked = false; // Only show 10-streak once
        private static bool hundredUnlocked = false; // Only show 100-answered once

        public void Start()
        {
            Console.WriteLine($"\nStarting game in {difficulty} mode. Type 'stop' to end the session at any time.\n");

            int questionIndex = 0;
            var used = new HashSet<int>(); // 避免重复

            while (used.Count < questions.Count)
            {
                // 随机找一个没问过的题目
                int index;
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
                    if (streak > 5 && streak < 20)
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
                    if (streak >= 5)
                        Console.WriteLine($"💧 Streak broken! You were on a {streak} answer streak. Keep going!");
                    streak = 0;
                    Console.WriteLine($"❌Wrong! Correct answer: {q.Answer}\n");
                }

                if (totalAnswered == 100 && !hundredUnlocked)
                {
                    Console.WriteLine("🌟 Achievement unlocked: 100 questions answered! True Geography Warrior!");
                    hundredUnlocked = true;
                }

            }

            ShowResults();
        }


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
            // 👇 更新历史数据
            totalQuestionsAnswered += correct + wrong;

            if (streak > maxStreak)
                maxStreak = streak;

            if (rate > maxAccuracy)
                maxAccuracy = rate;

            // 👇 成就反馈
            // 省略：rate 的幽默反馈...

            Console.WriteLine($"\n📊 Total questions answered so far: {totalQuestionsAnswered}");
            Console.WriteLine($"🏅 Highest streak ever: {maxStreak}");
            Console.WriteLine($"📈 Highest accuracy ever: {maxAccuracy:F2}%");

        }
    }

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
                Console.WriteLine("\n🎮 Select a game mode:\n");

                Console.WriteLine("  1. Country ➔ Capital");
                Console.WriteLine("     I’ll name a European country — you type its capital city.\n");

                Console.WriteLine("  2. City ➔ Country");
                Console.WriteLine("     I’ll give you a European city (not a capital) — you say which country it belongs to.\n");

                Console.WriteLine("  3. Feature ➔ Country");
                Console.WriteLine("     I’ll describe a European country with a fun fact or historical clue — you guess the country.\n");

                Console.WriteLine("  4. Largest Neighbor ➔ Country");
                Console.WriteLine("     I’ll name a country — you tell me its largest neighboring country.\n");

                Console.WriteLine("  5. Area Sort Challenge");
                Console.WriteLine("     I’ll give you several countries — sort them by area from smallest to largest.\n");

                Console.WriteLine("  6. Random Mixed Challenge");
                Console.WriteLine("     A mix of 1 and 2. If I give you a city, you type which country it belongs to; If I give you a country, you give me its capital city. They will appear randomly.\n");

                Console.WriteLine("  7. Change Difficulty");
                Console.WriteLine("     Switch between Easy / Medium / Hard mode at any time.\n");

                Console.WriteLine("  8. Exit Game");
                Console.WriteLine("     Leave the game — see you next time!\n");


                string key = Console.ReadLine();
                if (key == "8") break;

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
                            int count = difficulty switch
                            {
                                "Easy" => 4,
                                "Medium" => 5,
                                "Hard" => 6,
                                _ => 5
                            };

                            // 加载并随机抽取符合难度的题目
                            var areaQuestions = DataManager.LoadQuestions("data/area.txt")
                                .Where(q => q.Difficulty == difficulty)
                                .OrderBy(_ => Guid.NewGuid())
                                .Take(count)
                                .ToList();

                            // 使用 C# 内置排序按面积从小到大排序
                            areaQuestions = areaQuestions
                                .OrderBy(q => double.Parse(q.Answer.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture))
                                .ToList();

                            Console.WriteLine("\n🧠 Area Sort Challenge: Sort the following countries from smallest to largest area.\n");

                            var shuffled = areaQuestions.OrderBy(_ => Guid.NewGuid()).ToList();
                            foreach (var q in shuffled)
                            {
                                Console.WriteLine($" - {q.Prompt}");
                            }

                            Console.WriteLine("\nEnter your sorted list (comma separated):");
                            string input = Console.ReadLine();
                            var userOrder = input.Split(',').Select(s => s.Trim()).ToList();
                            var correctOrder = areaQuestions.Select(q => q.Prompt).ToList();

                            int correct = userOrder.Zip(correctOrder, (u, c) => u.Equals(c, StringComparison.OrdinalIgnoreCase) ? 1 : 0).Sum();
                            Console.WriteLine($"\n✅ You got {correct} out of {correctOrder.Count} in the correct order.");
                            Console.WriteLine("Correct order was:");
                            foreach (var q in correctOrder)
                                Console.WriteLine($" - {q}");
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine("❌ Error parsing area values. Please check your area.txt file.");
                        }
                        continue;
                    case "6":
                        try
                        {
                            // Merge all question types (filtered by current difficulty)
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

                            // Shuffle questions
                            mixedQuestions = mixedQuestions.OrderBy(_ => Guid.NewGuid()).ToList();

                            // Start session without name conflict
                            var mixedSession = new GameSession(mixedQuestions, difficulty);
                            mixedSession.Start();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("❌ Error loading questions: " + ex.Message);
                        }
                        continue;
                    case "7":
                        Console.WriteLine("\n🔧 Change difficulty level:");
                        Console.WriteLine("Enter new difficulty (Easy / Medium / Hard):");
                        string newDifficulty = Console.ReadLine().Trim();

                        if (new[] { "Easy", "Medium", "Hard" }.Contains(newDifficulty))
                        {
                            difficulty = newDifficulty;
                            Console.WriteLine($"✅ Difficulty updated to {difficulty}.");
                        }
                        else
                        {
                            Console.WriteLine("❌ Invalid difficulty. No change made.");
                        }
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
