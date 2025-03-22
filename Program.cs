internal class Program
{
    // TODO filters in listbalance syntax list balance sort_by="value" sort="highest", list balance sort_by="value" sort="lowest"
    // TODO filter by category
    // TODO filter by date - last month, last week, last year
    // TODO filter by amount less / most

    // TODO add a way to add many transactions at once
    // TODO add a way to remove transactions
    // TODO add a way to edit transactions
    // TODO add a way to save transactions to a file
    // TODO add a way to load transactions from a file
    
    private static readonly List<Transaction> TransactionsList = new();

    private static void IntroductionMessage()
    {
        Console.WriteLine("Willkommen beim Budgetierungsprogramm! Gebe 'exit' zum Verlassen oder 'help' für eine Liste aller Commands ein.");   
    }
    
    private static void CommandOverview()
    {
        Console.WriteLine("expense add <Betrag> <Kategorie> <Beschreibung>");
        Console.WriteLine("income add <Betrag> <Kategorie> <Beschreibung>");
        Console.WriteLine("show_transactions - um alle Transaktionen anzuzeigen");
        Console.WriteLine("balance - um dein Guthaben anzuzeigen");
        Console.WriteLine("clear - um den Bildschirm zu löschen");
    }

    private static void Main()
    {
        CommandOverview();
        Console.WriteLine();
        while (true)
        {
            var command = Console.ReadLine();
            HandleCommand(command);
        }
    }
    
    private static void HandleCommand(string? command)
    {
        string? trimmedCommand = command?.Trim().ToLower();
        string[]? commandParts = trimmedCommand?.Split(' ');

        if (commandParts?.Contains("exit") == true) Environment.Exit(0);
        if (commandParts?.Contains("help") == true)
        {
            CommandOverview();
        };

        if (commandParts?.Contains("clear") == true)
        {
            Console.Clear();
            IntroductionMessage();
        }

        if (commandParts?.Contains("show_transactions") == true)
        {
            ShowTransactionList();
        }

        if (commandParts?.Contains("balance") == true)
        {
            ShowBalance();
        }

        bool hasOptionalDescription = commandParts?.Length == 5;
        bool hasAmountAndCategory = commandParts?.Length == 4;
        bool isCategoryMissing = commandParts?.Length == 3;
        bool isAmountMissing = commandParts?.Length == 2;
        
        if (commandParts?.Contains("expense") == true && commandParts.Contains("add"))
        {
            if (hasOptionalDescription)
            {
                string amount = commandParts[2];
                string category = commandParts[3];
                string description = commandParts[4];
                AddExpense(decimal.Parse(amount), category, description);
            }
            if (hasAmountAndCategory)
            {
                string amount = commandParts[2];
                string category = commandParts[3];
                AddExpense(decimal.Parse(amount), category);
            }
            else if(isCategoryMissing)
            {
                Console.WriteLine("Please enter a category for the expense.");
            } 
            else if (isAmountMissing)
            {
                Console.WriteLine("Please enter a amount for the expense.");
            }
        }
        
        if (!(commandParts?.Contains("income") == true && commandParts?.Contains("add") == true))
        {
            return;
        }

        if (isAmountMissing)
        {
            Console.WriteLine("Please enter a amount for the income.");
        } 
        else if(isCategoryMissing)
        {
            Console.WriteLine("Please enter a category for the income.");
        } 
        else
        {
            string amount = commandParts[2];
            string category = commandParts[3];
            AddIncome(decimal.Parse(amount), category);
        }
    }

    private static void AddExpense(decimal amount, string category)
    {
        AddExpense(amount, category, "");
    }
    
    private static void AddExpense(decimal amount, string category, string description)
    {
        var transaction = new Transaction(-amount, category, description);
        TransactionsList.Add(transaction);
        string outputMessage = $"Ausgabe wurde hinzugefügt: {amount}kr für {category}";
        if (!string.IsNullOrEmpty(description))
        {
            outputMessage += $" mit Beschreibung: {description}";
        }
        Console.WriteLine(outputMessage);
    }

    private static void AddIncome(decimal amount, string category)
    {
        var transaction = new Transaction(amount, category);
        TransactionsList.Add(transaction);
        Console.WriteLine($"Einnahme wurde hinzugefügt: {amount}kr für {category}");
    }

    private static void ShowBalance()
    {
        decimal totalIncome = 0;
        decimal totalExpense = 0;

        for (var i = 0; i < TransactionsList.Count; i++)
            if (TransactionsList[i].Amount > 0)
                totalIncome += TransactionsList[i].Amount;
            else
                totalExpense += TransactionsList[i].Amount;

        var balance = totalIncome + totalExpense;
        Console.WriteLine("Gesamte Einnahmen: " + totalIncome + "kr");
        Console.WriteLine("Gesamte Ausgaben: " + totalExpense + "kr");
        Console.WriteLine("Dein Kontostand ist: " + balance + "kr");
    }

    private static void ShowTransactionList()
    {
        if (TransactionsList.Count == 0)
        {
            Console.WriteLine("There are no Transactions to show a balance for.");
        }
        else
        { 
            for (var i = 0; i < TransactionsList.Count; i++){
                Console.WriteLine(
                    $"{TransactionsList[i].Date} {TransactionsList[i].Amount}kr {TransactionsList[i].Category}");
            }
        }
    }
}