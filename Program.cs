using System.Net.Http.Headers;
using System.Runtime.CompilerServices;


//Utility
const ConsoleColor defaultTextColor = ConsoleColor.Yellow;


const int targetDiceCount = 2;
int[] targetDice = new int[targetDiceCount];

int playerHP = 0;
int targetHP = targetDiceCount;

int fightLength;
bool fightComplete;
char loadingWidgetChar;
bool canRun = true;

string greeting = "Welcome to the nicey dicey lousey housey of dice war rpg fun.";
string welcomeMessage = "Each round you will be given a random number of dice to beat the 'two' target dice.\n" +
                      "You win if any two or more of your dice are higher than both of the target dice.\n" +
                      "You lose if the target dice are higher than 80% of your dice (rounded up).\n" +
                      "You draw on any other condition and get to re-roll.\n\n" +
                      "Controls:\n" +
                      "Press (ctrl x) to exit at any time.\n" +
                      "Press (ctrl q) to return to this screen.\n" +
                      "Press 'Enter' or 'Spacebar' to play.\n\n" +
                      "Good Luck!";
string winMessage() => ($"Congratulations you won the fight with {playerHP}HP left!");
string loseMessage() => ($"My condolences, the target won the fight with {targetHP}HP left over.");
string drawMessage = ($"Through the misty fog of war, two opponents stood alone " +
                     $"'neath an ashen sky and amongst the gorey bones");
string playAgainMessage = ($"Care to take another go standing mano e mano with your foe?");

int[] RollPlayerDice(int[] playerDice)
{
    string result = "";
    foreach (int die in playerDice)
    {
        int index = Array.IndexOf(playerDice, die);
        playerDice[index] = new Random().Next(1, 7);
        string.Join($"Die #{index}: ", result, playerDice[index]);
    }

    Console.WriteLine($"You were given {playerDice.Length} dice and rolled the following:\n{result}");

    return playerDice;
}
void RollTargetDice()
{
    string result = "";
    foreach (int die in targetDice)
    {
        int index = Array.IndexOf(targetDice, die);
        targetDice[index] = new Random().Next(1, 6);
        string.Join($"Die #{index}: ", result, targetDice[index]);
    }

    Console.WriteLine($"The Target rolled the following:\n{result}");
}


//Game Logic
void Fight(int[] playerDice)
{
    bool success = false;
    bool isDraw = false;

    targetHP = targetDiceCount;
    playerHP = Convert.ToInt32(playerDice.Length * 0.8);

    RollPlayerDice(playerDice);
    RollTargetDice();

    foreach (int targetDie in targetDice)
    {
        bool isTargetHit = false;
        bool isPlayerHit = false;
        foreach (int playerDie in playerDice)
        {
            if (playerDie > targetDie)
                isTargetHit = true;
            if (playerDie < targetDie)
                isPlayerHit = true;
        }

        if (isTargetHit)
        {
            targetHP--;
            if (targetHP <= 0)
                success = true;
        }
        if (isPlayerHit)
        {
            playerHP--;
            if (playerHP <= 0)
                success = false;
        }

        isDraw = (playerHP > 0) && (targetHP > 0);
    }

    DisplayFightResults(success, isDraw);
}


//User Interface
void DisplayFightTimer()
{
    Console.Clear();

    int cursorLeft = Console.CursorLeft;
    int interval = Convert.ToInt32(fightLength / 20);

    while (!fightComplete)
    {
        PulseLoadingWidget(cursorLeft);
        Task.Delay(500).Wait();
    }
}
void PulseLoadingWidget(int cursorLeft)
{
    Console.CursorLeft = cursorLeft;
    if (loadingWidgetChar != '/' && loadingWidgetChar != '\\')
    {
        Console.Write($"Engaging... \\");
        loadingWidgetChar = '/';
    }
    else if (loadingWidgetChar == '/')
    {
        Console.Write($"Engaging... /");
        loadingWidgetChar = '\\';
    }
    else
    {
        Console.Write($"Engaging... \\");
        loadingWidgetChar = '/';
    }

    fightLength--;
    if (fightLength <= 0)
        fightComplete = true;
}
void DisplayHorizontalLine(ConsoleColor color, int length)
{
    Console.ForegroundColor = color;
    Console.WriteLine("".PadRight(length, '~'));
    Console.ForegroundColor = defaultTextColor;
}
void DisplayWelcomeMessage()
{
    Console.Clear();

    int longestSentenceLength = 0;
    foreach (string sentence in welcomeMessage.Split('.'))
    {
        if (sentence.Length > longestSentenceLength)
            longestSentenceLength = sentence.Length;
    }

    DisplayHorizontalLine(ConsoleColor.Green, longestSentenceLength);

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(greeting);
    Console.ForegroundColor = defaultTextColor;

    DisplayHorizontalLine(ConsoleColor.Green, longestSentenceLength);

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(welcomeMessage);
    Console.ForegroundColor = defaultTextColor;

    DisplayHorizontalLine(ConsoleColor.Green, longestSentenceLength);
}
void DisplayMessage(string message)
{
    Console.Clear();
    DisplayHorizontalLine(ConsoleColor.DarkGreen, message.Length);

    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine(message);
    Console.ForegroundColor = defaultTextColor;

    DisplayHorizontalLine(ConsoleColor.DarkGreen, message.Length);
}
bool PromptToPlayAgian()
{
    DisplayHorizontalLine(ConsoleColor.DarkGray, playAgainMessage.Length);
    Console.WriteLine(playAgainMessage);
    Console.WriteLine("Y|N");
    ConsoleKeyInfo response = Console.ReadKey();
    switch (response.Key)
    {
        case ConsoleKey.Y:
            return true;
        case ConsoleKey.N:
            return false;
        case ConsoleKey.X:
            if (response.Modifiers == ConsoleModifiers.Control)
                Quit();
            return false;
        case ConsoleKey.Q:
            if (response.Modifiers == ConsoleModifiers.Control)
                Run();
            return false;
        default:
            return false;
    }
}
void DisplayFightResults(bool success, bool isDraw)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    if (success && !isDraw)
        DisplayMessage(winMessage());
    else if (!success && !isDraw)
        DisplayMessage(loseMessage());
    else if (isDraw)
        DisplayMessage(drawMessage);
    Console.ForegroundColor = defaultTextColor;
}


//Application Logic
void Reset()
{
    fightLength = new Random().Next(3, 10);
    fightComplete = false;
    loadingWidgetChar = '|';
}
void Quit()
{
    Environment.Exit(0);
}
void StartFight()
{
    do
    {
        DisplayFightTimer();
        Fight(new int[new Random().Next(2, 8)]);

    } while (!fightComplete);
}
void Run()
{
    Reset();

    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = defaultTextColor;

#pragma warning disable CA1416
    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        Console.WindowWidth = 169;
#pragma warning restore CA1416

    DisplayWelcomeMessage();

    ConsoleKeyInfo command = new ConsoleKeyInfo();
    while (command.Key != ConsoleKey.Enter && command.Key != ConsoleKey.Spacebar)
    {
        command = Console.ReadKey();
        if (command.Modifiers == ConsoleModifiers.Control)
            if (command.Key == ConsoleKey.X)
                Quit();
            else if (command.Key == ConsoleKey.Q)
                Run();
    }

    Console.Clear();

    StartFight();

    //Inner Application Loop
    while (PromptToPlayAgian())
    {
        Reset();
        StartFight();
    }
}

//Trigger
try
{
    //Main Application Loop
    while (canRun)
        Run();
}
catch(InvalidOperationException ex)
{
    canRun = false;
}
catch (Exception ex)
{
    Console.WriteLine("");

    DisplayHorizontalLine(ConsoleColor.Magenta, ex.Message.Length);

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex);
    Console.ForegroundColor = defaultTextColor;

    DisplayHorizontalLine(ConsoleColor.Magenta, ex.Message.Length);
}