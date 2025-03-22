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
        Console.WriteLine();
        Console.WriteLine("Welcome to the budgeting program! Type 'exit' to quit or 'help' for a list of all commands.");   
    }
    
    private static void CommandOverview()
    {
        Console.WriteLine("expense add <Amount> <Category> <Description>");
        Console.WriteLine("income add <Amount> <Category> <Description>");
        Console.WriteLine("show_transactions - to display all transactions");
        Console.WriteLine("balance - to show your current balance");
        Console.WriteLine("clear - to clear the screen");
    }

    private static void Main()
    {
        IntroductionMessage();
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
                Console.WriteLine("Please enter an amount for the expense.");
            }
        }
        
        if (!(commandParts?.Contains("income") == true && commandParts?.Contains("add") == true))
        {
            return;
        }

        if (isAmountMissing)
        {
            Console.WriteLine("Please enter an amount for the income.");
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
        string outputMessage = $"Expense added: {amount}kr for {category}";
        if (!string.IsNullOrEmpty(description))
        {
            outputMessage += $" with description: {description}";
        }
        Console.WriteLine(outputMessage);
    }

    private static void AddIncome(decimal amount, string category)
    {
        var transaction = new Transaction(amount, category);
        TransactionsList.Add(transaction);
        Console.WriteLine($"Income added: {amount}kr for {category}");
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
        Console.WriteLine("Total income: " + totalIncome + "kr");
        Console.WriteLine("Total expenses: " + totalExpense + "kr");
        Console.WriteLine("Your account balance is: " + balance + "kr");
    }

    private static void ShowTransactionList()
    {
        if (TransactionsList.Count == 0)
        {
            Console.WriteLine("There are no transactions to show a balance for.");
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