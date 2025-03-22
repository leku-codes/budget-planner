using System;
using System.Collections.Generic;

public struct Transaction
{
    private static int Id = 0;
    public decimal Amount;
    public string Category;
    public string Description;
    public DateOnly Date;
    public TimeOnly Time;
    
    public Transaction(decimal amount, string category) : this(amount, category, "")
    {
    }
    
    public Transaction(decimal amount, string category, string description)
    {
        Id = generateId();
        Date = DateOnly.FromDateTime(DateTime.Now);
        Time = TimeOnly.FromDateTime(DateTime.Now);
        Amount = amount;
        Category = category;
        Description = description;
    }
    
    static int generateId()
    {
        return Id++;
    }
}