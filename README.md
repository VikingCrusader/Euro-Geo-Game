Identification
Name, surname, xname: Yiwen Zhang, xzhay009

Email: xzhay009@studenti.czu.cz

Year of study, study programme: 1rd year, 2nd semester, Bachelor of Informatics

Name / Topic
Project Name: Euro Geo Game

Topic: European Geography Quiz Console Game

Description
This program is a console-based quiz game that helps users test and improve their knowledge of European geography. Users can choose different difficulty levels and quiz modes, answer questions, receive instant feedback, and unlock achievements based on their performance.

Data structures / objects / files
Class Question: Represents one quiz item with properties for prompt, answer, and difficulty level.

Class GameSession: Manages one quiz round, handles gameplay logic, score tracking, and achievements.

Class DataManager: Loads questions from .txt files into List<Question>.

Files used:

data/capitals.txt – Country → Capital questions

data/cities.txt – City → Country questions

data/features.txt – Fun facts or descriptions → Country

data/largest_neighbors.txt – Country → Largest neighbor

All files are formatted as Prompt;Answer;Difficulty

Functionality
Users can:

Choose a difficulty: Easy, Medium, or Hard (cases and spaces are not sensitive)

Select quiz mode from a menu:

Country ➔ Capital
The user is given the name of a European country and must type its capital city.
City ➔ Country
The user is shown the name of a European city (not a capital), and must answer which country it belongs to.
Feature ➔ Country
The game presents a unique fun fact, geographic trait, or historical clue. The user guesses which country is being described.
Largest Neighbor ➔ Country
The game gives a European country, and the user has to answer which of its neighbors has the largest land area.
Mixed Challenge (random country/city mix)
Combines “Country ➔ Capital” and “City ➔ Country” questions randomly. This mode provides more variety and unpredictability.
Change difficulty
Allows the user to switch between Easy, Medium, and Hard question sets at any time during the game.

Exit game

The game randomly selects non-repeating questions
Shaffle: OrderBy(\_ => Guid.NewGuid()).
Non-repeating:HashSet<int>
Answers are checked case- and space-insensitively.

Score and accuracy are shown at the end of each session.

Two achievements:

Answering 100 total questions

Streaks of correct answers (with special feedback at 10+, 20+, 50+)

Issues / input checking / problems that may occur
If question files are missing or corrupt, the program throws an error with a message.

If the user types an invalid difficulty or menu option, it defaults or repeats the prompt.

Answers are trimmed and lowercased to avoid false mismatches.

Typing "stop" at any time ends the session early without crashing.

Repeated questions in a session are avoided using a HashSet.
