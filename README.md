# Budget Planner

A simple command-line tool to manage your personal finances.

## Introduction

Budget Planner helps you track your income and expenses to maintain an overview of your financial situation.

## Available Commands

### General Commands
| Command | Description |
|---------|-------------|
| `help` | Displays a list of all available commands |
| `clear` | Clears the entire log and displays the introduction message |

### Financial Management

#### Income
```
income add <amount> <category> [description]
```
Adds a new income transaction.
- `<amount>`: The amount of money (e.g., 1000.50)
- `<category>`: The income category (e.g., Salary, Side job, Gift)
- `[description]`: Optional - A brief description (e.g., "January monthly salary")

```
income remove <transactionId>
```
Removes an existing income transaction.
- `<transactionId>`: The ID of the transaction to delete

#### Expenses
```
expense add <amount> <category> [description]
```
Adds a new expense transaction.
- `<amount>`: The amount of money (e.g., 24.99)
- `<category>`: The expense category (e.g., Groceries, Rent, Transportation)
- `[description]`: Optional - A brief description (e.g., "Weekly shopping")

```
expense remove <transactionId>
```
Removes an existing expense transaction.
- `<transactionId>`: The ID of the transaction to delete

#### Analysis
```
balance
```
Displays the current account balance (income - expenses).

```
show_transactions [count] [category]
```
Displays a list of all transactions.
- `[count]`: Optional - Limits the number of transactions shown
- `[category]`: Optional - Filters transactions by category

## Examples

### Adding Income
```
income add 2000 Salary March monthly salary
```

### Adding an Expense
```
expense add 35.50 Groceries Shopping at Walmart
```

### Checking Balance
```
balance
```

### Viewing Last 5 Transactions
```
show_transactions 5
```

### Viewing All Transactions in "Groceries" Category
```
show_transactions Groceries
```

## Tips
- Use consistent category names to better organize your finances
- Regularly use the `balance` command to keep track of your financial situation
- Add detailed descriptions to help you remember what money was spent on later
- Consider categorizing expenses into essential (rent, food) and non-essential (entertainment) to identify potential savings
- Export your transaction data regularly for backup and additional analysis