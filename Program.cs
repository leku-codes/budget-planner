namespace BudgetPlanning;

// Basisinterface für alle Kommandos
public interface ICommand
{
    string Name { get; }
    string Description { get; }
    string Usage { get; }
    bool Execute(string[] args);
}

// Kommandoregistry zum Verwalten aller verfügbaren Kommandos
public class CommandRegistry
{
    private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);

    public void RegisterCommand(ICommand command)
    {
        _commands[command.Name] = command;
    }

    public bool TryGetCommand(string commandName, out ICommand command)
    {
        return _commands.TryGetValue(commandName, out command);
    }

    public IEnumerable<ICommand> GetAllCommands()
    {
        return _commands.Values;
    }
}

// Parser für Kommandoeingaben
public class CommandParser
{
    private readonly CommandRegistry _registry;

    public CommandParser(CommandRegistry registry)
    {
        _registry = registry;
    }

    public bool TryParseAndExecute(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        string[] parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length == 0)
            return false;

        string commandName = parts[0].ToLower();
        
        if (_registry.TryGetCommand(commandName, out ICommand command))
        {
            string[] args = parts.Skip(1).ToArray();
            return command.Execute(args);
        }
        
        Console.WriteLine($"Unbekanntes Kommando: {commandName}");
        return false;
    }
}

// Hauptklasse für die Anwendung
public class BudgetApp
{
    private readonly CommandRegistry _registry = new CommandRegistry();
    private readonly CommandParser _parser;
    private readonly List<Transaction> _transactions = new List<Transaction>();

    public BudgetApp()
    {
        _parser = new CommandParser(_registry);
        RegisterCommands();
    }

    private void RegisterCommands()
    {
        // Hier registrieren wir alle unsere Kommandos
        _registry.RegisterCommand(new HelpCommand(_registry));
        _registry.RegisterCommand(new ExitCommand());
        _registry.RegisterCommand(new ClearCommand());
        _registry.RegisterCommand(new ShowTransactionsCommand(_transactions));
        _registry.RegisterCommand(new BalanceCommand(_transactions));
        _registry.RegisterCommand(new AddExpenseCommand(_transactions));
        _registry.RegisterCommand(new AddIncomeCommand(_transactions));
        _registry.RegisterCommand(new DeleteTransactionCommand(_transactions));
        // Weitere Kommandos können hier einfach hinzugefügt werden
    }

    public void Run()
    {
        Console.WriteLine("Willkommen beim Budgetierungsprogramm!");
        Console.WriteLine("Geben Sie 'help' ein, um eine Liste der verfügbaren Befehle anzuzeigen.");

        while (true)
        {
            Console.Write("> ");
            string input = Console.ReadLine();
            _parser.TryParseAndExecute(input);
        }
    }
}

// Implementierungen der einzelnen Kommandos
public class HelpCommand : ICommand
{
    private readonly CommandRegistry _registry;

    public HelpCommand(CommandRegistry registry)
    {
        _registry = registry;
    }

    public string Name => "help";
    public string Description => "Zeigt Hilfe zu allen verfügbaren Befehlen an.";
    public string Usage => "help [Befehl]";

    public bool Execute(string[] args)
    {
        if (args.Length == 0)
        {
            // Zeige allgemeine Hilfe
            Console.WriteLine("Verfügbare Befehle:");
            foreach (var command in _registry.GetAllCommands())
            {
                Console.WriteLine($"  {command.Name,-15} - {command.Description}");
            }
            Console.WriteLine("\nGeben Sie 'help [Befehl]' ein, um detaillierte Hilfe zu einem bestimmten Befehl zu erhalten.");
        }
        else
        {
            // Zeige Hilfe für einen bestimmten Befehl
            string commandName = args[0];
            if (_registry.TryGetCommand(commandName, out ICommand command))
            {
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
}

public class ExitCommand : ICommand
{
    public string Name => "exit";
    public string Description => "Beendet das Programm.";
    public string Usage => "exit";

    public bool Execute(string[] args)
    {
        Console.WriteLine("Auf Wiedersehen!");
        Environment.Exit(0);
        return true;
    }
}

public class ClearCommand : ICommand
{
    public string Name => "clear";
    public string Description => "Löscht den Bildschirminhalt.";
    public string Usage => "clear";

    public bool Execute(string[] args)
    {
        Console.Clear();
        return true;
    }
}

public class ShowTransactionsCommand : ICommand
{
    private readonly List<Transaction> _transactions;

    public ShowTransactionsCommand(List<Transaction> transactions)
    {
        _transactions = transactions;
    }

    public string Name => "show";
    public string Description => "Zeigt alle Transaktionen an.";
    public string Usage => "show [filter=value]";

    public bool Execute(string[] args)
    {
        if (_transactions.Count == 0)
        {
            Console.WriteLine("Keine Transaktionen vorhanden.");
            return true;
        }

        // Hier können Filter implementiert werden
        IEnumerable<Transaction> filtered = _transactions;
        
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

    private void DisplayTransactions(IEnumerable<Transaction> transactions)
    {
        Console.WriteLine($"{"ID",-6} | {"Datum",-12} | {"Zeit",-8} | {"Betrag",-12} | {"Kategorie",-15} | {"Beschreibung",-30}");
        Console.WriteLine(new string('-', 85));
        
        foreach (var t in transactions)
        {
            Console.WriteLine($"{t.Id,-6} | {t.Date,-12} | {t.Time,-8} | {t.Amount,-10:F2}kr | {t.Category,-15} | {t.Description,-30}");
        }
    }
}

public class BalanceCommand : ICommand
{
    private readonly List<Transaction> _transactions;

    public BalanceCommand(List<Transaction> transactions)
    {
        _transactions = transactions;
    }

    public string Name => "balance";
    public string Description => "Zeigt den aktuellen Kontostand an.";
    public string Usage => "balance";

    public bool Execute(string[] args)
    {
        decimal totalIncome = _transactions.Where(t => t.Amount > 0).Sum(t => t.Amount);
        decimal totalExpense = _transactions.Where(t => t.Amount < 0).Sum(t => t.Amount);
        decimal balance = totalIncome + totalExpense;

        Console.WriteLine($"Gesamteinnahmen: {totalIncome:F2}kr");
        Console.WriteLine($"Gesamtausgaben: {totalExpense:F2}kr");
        Console.WriteLine($"Kontostand: {balance:F2}kr");
        
        return true;
    }
}

public class AddExpenseCommand : ICommand
{
    private readonly List<Transaction> _transactions;

    public AddExpenseCommand(List<Transaction> transactions)
    {
        _transactions = transactions;
    }

    public string Name => "expense";
    public string Description => "Fügt eine neue Ausgabe hinzu.";
    public string Usage => "expense <Betrag> <Kategorie> [Beschreibung]";

    public bool Execute(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Fehler: Zu wenige Argumente.");
            Console.WriteLine($"Verwendung: {Usage}");
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
        _transactions.Add(transaction);

        Console.WriteLine("Neue Ausgabe hinzugefügt:");
        Console.WriteLine($"{transaction.Id,-6} | {transaction.Date,-12} | {transaction.Time,-8} | {transaction.Amount,-10:F2}kr | {transaction.Category,-15} | {transaction.Description,-30}");
        
        return true;
    }
}

public class AddIncomeCommand : ICommand
{
    private readonly List<Transaction> _transactions;

    public AddIncomeCommand(List<Transaction> transactions)
    {
        _transactions = transactions;
    }

    public string Name => "income";
    public string Description => "Fügt eine neue Einnahme hinzu.";
    public string Usage => "income <Betrag> <Kategorie> [Beschreibung]";

    public bool Execute(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Fehler: Zu wenige Argumente.");
            Console.WriteLine($"Verwendung: {Usage}");
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
        _transactions.Add(transaction);

        Console.WriteLine("Neue Einnahme hinzugefügt:");
        Console.WriteLine($"{transaction.Id,-6} | {transaction.Date,-12} | {transaction.Time,-8} | {transaction.Amount,-10:F2}kr | {transaction.Category,-15} | {transaction.Description,-30}");
        
        return true;
    }
}

public class DeleteTransactionCommand : ICommand
{
    private readonly List<Transaction> _transactions;

    public DeleteTransactionCommand(List<Transaction> transactions)
    {
        _transactions = transactions;
    }

    public string Name => "delete";
    public string Description => "Löscht eine Transaktion anhand ihrer ID.";
    public string Usage => "delete <ID>";

    public bool Execute(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Fehler: Bitte eine ID angeben.");
            Console.WriteLine($"Verwendung: {Usage}");
            return false;
        }

        if (!int.TryParse(args[0], out int id))
        {
            Console.WriteLine("Fehler: Die ID muss eine Zahl sein.");
            return false;
        }
        
        var transaction = _transactions.FirstOrDefault(t => t.Id == id);
        if (transaction.Equals(default(Transaction)))
        {
            Console.WriteLine($"Fehler: Keine Transaktion mit ID {id} gefunden.");
            return false;
        }

        _transactions.Remove(transaction);
        Console.WriteLine($"Transaktion mit ID {id} wurde gelöscht.");
        
        return true;
    }
}

// Einstiegspunkt
class Program
{
    static void Main()
    {
        var app = new BudgetApp();
        app.Run();
    }
}