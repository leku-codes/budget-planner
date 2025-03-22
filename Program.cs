namespace BudgetPlanning;

internal struct Program
{
    // Kommandotypen als Enum
    private enum CommandType
    {
        Unknown,
        Help,
        Exit,
        Clear,
        ShowTransactions,
        Balance,
        AddExpense,
        AddIncome,
        DeleteTransaction
    }

    // Struktur für Kommando-Definitionen
    private struct CommandDefinition
    {
        public string Name;
        public string Description;
        public string Usage;
        public Func<string[], bool> ExecuteFunction;
    }

    private static readonly List<Transaction> Transactions = [];
    private static readonly Dictionary<CommandType, CommandDefinition> Commands = new Dictionary<CommandType, CommandDefinition>();

    private static void Main()
    {
        InitializeCommands();
        
        Console.WriteLine("Willkommen beim Budgetierungsprogramm!");
        Console.WriteLine("Geben Sie 'help' ein, um eine Liste der verfügbaren Befehle anzuzeigen.");
        Console.WriteLine();
        
        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine();
            ParseAndExecuteCommand(input);
        }
    }

    private static void InitializeCommands()
    {
        // Registriere alle Kommandos
        Commands[CommandType.Help] = new CommandDefinition
        {
            Name = "help",
            Description = "Zeigt Hilfe zu allen verfügbaren Befehlen an.",
            Usage = "help [Befehl]",
            ExecuteFunction = ExecuteHelpCommand
        };

        Commands[CommandType.Exit] = new CommandDefinition
        {
            Name = "exit",
            Description = "Beendet das Programm.",
            Usage = "exit",
            ExecuteFunction = _ => { Environment.Exit(0); return true; }
        };

        Commands[CommandType.Clear] = new CommandDefinition
        {
            Name = "clear",
            Description = "Löscht den Bildschirminhalt.",
            Usage = "clear",
            ExecuteFunction = _ => { Console.Clear(); return true; }
        };

        Commands[CommandType.ShowTransactions] = new CommandDefinition
        {
            Name = "show",
            Description = "Zeigt alle Transaktionen an, mit optionalen Filtern.",
            Usage = "show [category=Wert] [sort=highest|lowest] [date=week|month]",
            ExecuteFunction = ExecuteShowTransactionsCommand
        };

        Commands[CommandType.Balance] = new CommandDefinition
        {
            Name = "balance",
            Description = "Zeigt den aktuellen Kontostand an.",
            Usage = "balance",
            ExecuteFunction = ExecuteBalanceCommand
        };

        Commands[CommandType.AddExpense] = new CommandDefinition
        {
            Name = "expense",
            Description = "Fügt eine neue Ausgabe hinzu.",
            Usage = "expense <Betrag> <Kategorie> [Beschreibung]",
            ExecuteFunction = ExecuteAddExpenseCommand
        };

        Commands[CommandType.AddIncome] = new CommandDefinition
        {
            Name = "income",
            Description = "Fügt eine neue Einnahme hinzu.",
            Usage = "income <Betrag> <Kategorie> [Beschreibung]",
            ExecuteFunction = ExecuteAddIncomeCommand
        };

        Commands[CommandType.DeleteTransaction] = new CommandDefinition
        {
            Name = "delete",
            Description = "Löscht eine Transaktion anhand ihrer ID.",
            Usage = "delete <ID>",
            ExecuteFunction = ExecuteDeleteTransactionCommand
        };
    }

    private static void ParseAndExecuteCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return;

        string[] parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length == 0)
            return;

        string commandName = parts[0].ToLower();
        CommandType commandType = GetCommandType(commandName);

        if (commandType == CommandType.Unknown)
        {
            Console.WriteLine($"Unbekanntes Kommando: {commandName}");
            return;
        }

        string[] args = parts.Skip(1).ToArray();
        Commands[commandType].ExecuteFunction(args);
    }

    private static CommandType GetCommandType(string commandName)
    {
        foreach (var kvp in Commands)
        {
            if (kvp.Value.Name.Equals(commandName, StringComparison.OrdinalIgnoreCase))
                return kvp.Key;
        }
        return CommandType.Unknown;
    }

    // Implementierungen der Kommando-Ausführungsfunktionen
    private static bool ExecuteHelpCommand(string[] args)
    {
        if (args.Length == 0)
        {
            // Zeige allgemeine Hilfe
            Console.WriteLine("Verfügbare Befehle:");
            foreach (var command in Commands.Values)
            {
                Console.WriteLine($"  {command.Name,-15} - {command.Description}");
            }
            Console.WriteLine("\nGeben Sie 'help [Befehl]' ein, um detaillierte Hilfe zu einem bestimmten Befehl zu erhalten.");
        }
        else
        {
            // Zeige Hilfe für einen bestimmten Befehl
            string commandName = args[0];
            CommandType commandType = GetCommandType(commandName);
            
            if (commandType != CommandType.Unknown)
            {
                var command = Commands[commandType];
                Console.WriteLine($"Befehl: {command.Name}");
                Console.WriteLine($"Beschreibung: {command.Description}");
                Console.WriteLine($"Verwendung: {command.Usage}");
            }
            else
            {
                Console.WriteLine($"Unbekannter Befehl: {commandName}");
            }
        }
        return true;
    }

    private static bool ExecuteShowTransactionsCommand(string[] args)
    {
        if (Transactions.Count == 0)
        {
            Console.WriteLine("Keine Transaktionen vorhanden.");
            return true;
        }

        // Hier können Filter implementiert werden
        IEnumerable<Transaction> filtered = Transactions;
        
        if (args.Length > 0)
        {
            // Filtern nach verschiedenen Kriterien
            foreach (var arg in args)
            {
                if (arg.StartsWith("category=", StringComparison.OrdinalIgnoreCase))
                {
                    string category = arg.Substring("category=".Length);
                    filtered = filtered.Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
                }
                else if (arg.StartsWith("sort=", StringComparison.OrdinalIgnoreCase))
                {
                    string sortType = arg.Substring("sort=".Length);
                    if (sortType.Equals("highest", StringComparison.OrdinalIgnoreCase))
                    {
                        filtered = filtered.OrderByDescending(t => t.Amount);
                    }
                    else if (sortType.Equals("lowest", StringComparison.OrdinalIgnoreCase))
                    {
                        filtered = filtered.OrderBy(t => t.Amount);
                    }
                }
                else if (arg.StartsWith("date=", StringComparison.OrdinalIgnoreCase))
                {
                    string dateFilter = arg.Substring("date=".Length);
                    DateTime now = DateTime.Now;
                    
                    if (dateFilter.Equals("week", StringComparison.OrdinalIgnoreCase))
                    {
                        DateOnly weekAgo = DateOnly.FromDateTime(now.AddDays(-7));
                        filtered = filtered.Where(t => t.Date >= weekAgo);
                    }
                    else if (dateFilter.Equals("month", StringComparison.OrdinalIgnoreCase))
                    {
                        DateOnly monthAgo = DateOnly.FromDateTime(now.AddMonths(-1));
                        filtered = filtered.Where(t => t.Date >= monthAgo);
                    }
                }
            }
        }

        DisplayTransactions(filtered);
        return true;
    }

    private static void DisplayTransactions(IEnumerable<Transaction> transactions)
    {
        Console.WriteLine($"{"ID",-6} | {"Datum",-12} | {"Zeit",-8} | {"Betrag",-12} | {"Kategorie",-15} | {"Beschreibung",-30}");
        Console.WriteLine(new string('-', 85));
        
        foreach (var t in transactions)
        {
            Console.WriteLine($"{t.Id,-6} | {t.Date,-12} | {t.Time,-8} | {t.Amount,-10:F2}kr | {t.Category,-15} | {t.Description,-30}");
        }
    }

    private static bool ExecuteBalanceCommand(string[] args)
    {
        decimal totalIncome = Transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
        decimal totalExpense = Transactions.Where(t => t.Amount < 0).Sum(t => t.Amount);
        decimal balance = totalIncome + totalExpense;

        Console.WriteLine($"Gesamteinnahmen: {totalIncome:F2}kr");
        Console.WriteLine($"Gesamtausgaben: {totalExpense:F2}kr");
        Console.WriteLine($"Kontostand: {balance:F2}kr");
        
        return true;
    }

    private static bool ExecuteAddExpenseCommand(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Fehler: Zu wenige Argumente.");
            Console.WriteLine($"Verwendung: expense <Betrag> <Kategorie> [Beschreibung]");
            return false;
        }

        if (!decimal.TryParse(args[0], out decimal amount))
        {
            Console.WriteLine("Fehler: Der Betrag muss eine Zahl sein.");
            return false;
        }

        string category = args[1];
        string description = args.Length > 2 ? string.Join(" ", args.Skip(2)) : "";

        var transaction = new Transaction(-Math.Abs(amount), category, description);
        Transactions.Add(transaction);

        Console.WriteLine("Neue Ausgabe hinzugefügt:");
        Console.WriteLine($"{transaction.Id,-6} | {transaction.Date,-12} | {transaction.Time,-8} | {transaction.Amount,-10:F2}kr | {transaction.Category,-15} | {transaction.Description,-30}");
        
        return true;
    }

    private static bool ExecuteAddIncomeCommand(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Fehler: Zu wenige Argumente.");
            Console.WriteLine($"Verwendung: income <Betrag> <Kategorie> [Beschreibung]");
            return false;
        }

        if (!decimal.TryParse(args[0], out decimal amount))
        {
            Console.WriteLine("Fehler: Der Betrag muss eine Zahl sein.");
            return false;
        }

        string category = args[1];
        string description = args.Length > 2 ? string.Join(" ", args.Skip(2)) : "";

        var transaction = new Transaction(Math.Abs(amount), category, description);
        Transactions.Add(transaction);

        Console.WriteLine("Neue Einnahme hinzugefügt:");
        Console.WriteLine($"{transaction.Id,-6} | {transaction.Date,-12} | {transaction.Time,-8} | {transaction.Amount,-10:F2}kr | {transaction.Category,-15} | {transaction.Description,-30}");
        
        return true;
    }

    private static bool ExecuteDeleteTransactionCommand(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Fehler: Bitte eine ID angeben.");
            Console.WriteLine($"Verwendung: delete <ID>");
            return false;
        }

        if (!int.TryParse(args[0], out int id))
        {
            Console.WriteLine("Fehler: Die ID muss eine Zahl sein.");
            return false;
        }

        var transaction = Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction.Equals(default(Transaction)))
        {
            Console.WriteLine($"Fehler: Keine Transaktion mit ID {id} gefunden.");
            return false;
        }

        Transactions.Remove(transaction);
        Console.WriteLine($"Transaktion mit ID {id} wurde gelöscht.");
        
        return true;
    }
}