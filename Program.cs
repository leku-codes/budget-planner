namespace BudgetPlanning;

internal class Program
{
    // TODO filters in show_transactions sort_by="value" sort="highest", show_transactions sort_by="value" sort="lowest"
    // TODO filter by category
    // TODO filter by date - last month, last week, last year
    // TODO filter by amount less / most

    // TODO add a way to add many transactions at once
    // TODO add a way to remove transactions
    // TODO add a way to edit transactions
    // TODO add a way to save transactions to a file
    // TODO add a way to load transactions from a file

    private static readonly List<Transaction> TransactionsList = [];
    
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
        if (string.IsNullOrWhiteSpace(command))
        {
            return;
        }

        string trimmedCommand = command.Trim().ToLower();
        string[] commandParts = trimmedCommand.Split(' ');

        bool isExitCommand = commandParts.Contains("exit");
        bool isHelpCommand = commandParts.Contains("help");
        bool isShowTransactionsCommand = commandParts.Contains("show_transactions");
        bool isClearCommand = commandParts.Contains("clear");
        bool isBalanceCommand = commandParts.Contains("balance");
        bool isExpenseCommand = commandParts.Contains("expense");
        bool isIncomeCommand = commandParts.Contains("income");
        bool isAddCommand = commandParts.Contains("add");
        bool isSortByCommand = commandParts.Contains("sort_by");

        if (isExitCommand) Environment.Exit(0);
        if (isHelpCommand)
        {
            CommandOverview();
        }

        if (isClearCommand)
        {
            Console.Clear();
            IntroductionMessage();
        }

        if (isShowTransactionsCommand)
        {
            ShowTransactionList();
        }

        if (isBalanceCommand)
        {
            ShowBalance();
        }

        bool hasOptionalDescription = commandParts.Length == 5;
        bool hasAmountAndCategory = commandParts.Length == 4;
        bool isCategoryMissing = commandParts.Length == 3;
        bool isAmountMissing = commandParts.Length == 2;

        if (isExpenseCommand && isAddCommand)
        {
            if (hasOptionalDescription)
            {
                string amount = commandParts[2];
                string category = commandParts[3];
                string description = commandParts[4];
                AddExpense(decimal.Parse(amount), category, description);
            }
            else if (hasAmountAndCategory)
            {
                string amount = commandParts[2];
                string category = commandParts[3];
                AddExpense(decimal.Parse(amount), category);
            }
            else if (isCategoryMissing)
            {
                Console.WriteLine("Please enter a category for the expense.");
            }
            else if (isAmountMissing)
            {
                Console.WriteLine("Please enter an amount for the expense.");
            }
        }

        bool isIncomeAdd = isIncomeCommand && isAddCommand;
        if (!isIncomeAdd)
        {
            return;
        }

        if (isAmountMissing)
        {
            Console.WriteLine("Please enter an amount for the income.");
        }
        else if (isCategoryMissing)
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
        Transaction transaction = new Transaction(-amount, category, description);
        TransactionsList.Add(transaction);
        LineSeparatedHeadline("Added new expense record:");
        TableLogSetup();
        NewTransactionLog(transaction);
    }

    private static void AddIncome(decimal amount, string category)
    {
        AddIncome(amount, category, "");
    }

    private static void AddIncome(decimal amount, string category, string description)
    {
        Transaction transaction = new Transaction(-amount, category, description);
        TransactionsList.Add(transaction);
        TableLogSetup();
        NewTransactionLog(transaction);
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
            Console.WriteLine("There are no transactions to show.");
        }
        else
        {
            TableLogSetup();
            for (var i = 0; i < TransactionsList.Count; i++)
            {
                Console.WriteLine($"{TransactionsList[i].Id,-6} | {TransactionsList[i].Date,-12} | {TransactionsList[i].Time,-8} | {TransactionsList[i].Amount,-10:F2}kr | {TransactionsList[i].Category,-15} | {TransactionsList[i].Description,-30}");
            }
            Console.WriteLine();
        }
    }

    private static void TableLogSetup()
    {
        string tableCols = $"{"ID",-6} | {"Date",-12} | {"Time",-8} | {"Amount",-12} | {"Category",-15} | {"Description",-30}";
        string dashedSeparator = new string('-', 85);
        Console.WriteLine(tableCols);
        Console.WriteLine(dashedSeparator);
    }

    private static void LineSeparatedHeadline(string headline)
    {
        Console.WriteLine();
        Console.WriteLine(headline);
        Console.WriteLine();
    }
    
    private static void NewTransactionLog(Transaction transaction)
    {
        string outputMessage = $"{transaction.Id,-6} | {transaction.Date,-12} | {transaction.Time,-8} | {transaction.Amount,-10:F2}kr | {transaction.Category,-15}";
        if (!string.IsNullOrEmpty(transaction.Description))
        {
            outputMessage +=
                $" | {transaction.Description,-30}";
        }

        Console.WriteLine(outputMessage);
    }
    
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

}