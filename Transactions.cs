public struct Transaction
{
    private static int _nextId = 1;
    public readonly int Id;
    public readonly decimal Amount;
    public readonly string Category;
    public readonly string Description;
    public DateOnly Date;
    public TimeOnly Time;
    
    public Transaction(decimal amount, string category) : this(amount, category, "")
    {
    }
    
    public Transaction(decimal amount, string category, string description)
    {
        Id = _nextId++;
        Date = DateOnly.FromDateTime(DateTime.Now);
        Time = TimeOnly.FromDateTime(DateTime.Now);
        Amount = amount;
        Category = category;
        Description = description;
    }
}