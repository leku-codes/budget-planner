using System;
using System.Collections.Generic;

public struct Transaction
{
    private static int Id = 0;
    public decimal Amount;
    public string Category;
    public string Description;
    public DateTime Date;
    
    public Transaction(decimal amount, string category) : this(amount, category, "")
    {
    }
    
    public Transaction(decimal amount, string category, string description)
    {
        Amount = amount;
        Category = category;
        Description = description;
        Date = DateTime.Now;
    }
    
    static int generateId()
    {
        return Id++;
    }
}