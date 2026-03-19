using tracker.Interfaces;
using tracker.Models;
using tracker.Services;
using tracker.Utilities;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

var serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);
var serviceProvider = serviceCollection.BuildServiceProvider();
var _hiyouService = serviceProvider.GetService<IHiyouServices>();

DisplayWelcomeMessage();
List<string> commands = [];

while (true) {
    Utility.PrintCommandMessage("Enter command: ");
    string input = Console.ReadLine() ?? string.Empty;

    if (string.IsNullOrEmpty(input)) {
        Utility.PrintInfoMessage("\n No input detected. Please try again.");
        continue;
    }

    commands = Utility.ParseInput(input);
    string firstCommand = commands[0].ToLower();
    bool yameruka = false;

    switch (firstCommand) {
        case "add":
            HiyouTennka();
            break;

        case "updatedes":
            HiyouDescriptionKoushinn();
            break;

        case "updateamo":
            HiyouAmountKoushinn();
            break;

        case "delete":
            HiyouSakujyou();
            break;

        case "list":
            ListHiyous();
            break;
            
        case "count":
            countAllHiyous();
            break;

        case "specific":
            countSpecificHiyou();
            break;

        case "help":
            DisplayAllCommands();
            break;

        case "exit":
            yameruka = true;
            break;

        case "clear":
            Utility.ClearConsole();
            DisplayWelcomeMessage();
            continue;

        default:
            break;  
    }

    if (yameruka)
        break;
}

void HiyouTennka() {
    if (!IsInputValid(commands, 3))
        return;

    if (double.TryParse(commands[2], out double amount) || amount < 0) {
        var addedID = _hiyouService?.addHiyou(amount, commands[1]);

        if (addedID != 0)
            Utility.PrintInfoMessage($"Hiyou added successfully (ID: {addedID})");
        else
            Utility.PrintInfoMessage("Failed at adding.");
            
    } else {
        Utility.PrintErrorMessage("Wrong command. Please try again.");
        Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
        return;
    }
}

void HiyouDescriptionKoushinn() {
    if (!IsInputValid(commands, 3))
        return;

    if (int.TryParse(commands[1], out int id)) {
        bool dekitaka = _hiyouService?.updateDescription(id, commands[2]) ?? false;

        if (dekitaka)
            Utility.PrintInfoMessage($"Hiyou updated successfully (ID: {id})");
        else
            Utility.PrintInfoMessage($"ID: {id}, does not exist.");
            
    } else {
        Utility.PrintErrorMessage("Wrong command. Please try again.");
        Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
        return;
    }
}

void HiyouAmountKoushinn() {
    if (!IsInputValid(commands, 3))
        return;

    if (int.TryParse(commands[1], out int id) && double.TryParse(commands[2], out double amount)) {
        if (amount >= 0) {
            var dekitaka = _hiyouService?.updateAmount(id, amount);

            if (dekitaka != null && dekitaka.Value)
                Utility.PrintInfoMessage($"Hiyou updated successfully (ID: {id})");
            else
                Utility.PrintInfoMessage($"ID: {id}, does not exist.");
        }   
    } else {
        Utility.PrintErrorMessage("Wrong command. Please try again.");
        Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
        return;
    }
}

void HiyouSakujyou() {
    if (!IsInputValid(commands, 2))
        return;

    if (int.TryParse(commands[1], out int id)) {
        var dekitaka = _hiyouService?.deleteHiyou(id);

        if (dekitaka != null && dekitaka.Value)
            Utility.PrintInfoMessage($"Hiyou deleted successfully (ID: {id})");
        else
            Utility.PrintInfoMessage($"ID: {id}, does not exist.");
            
    } else {
        Utility.PrintErrorMessage("Wrong command. Please try again.");
        Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
        return;
    }
}

void ListHiyous() {
    if (commands.Count > 1) {
        Utility.PrintErrorMessage("Wrong command. Please try again.");
        Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
        return;
    }
 
    var risuto = _hiyouService?.getAllHiyouRisuto() ?? [];
    
    DisplayHiyouAsTable(risuto);
}

void countAllHiyous() {
    if (commands.Count > 1) {
        Utility.PrintErrorMessage("Wrong command. Please try again.");
        Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
        return;
    }

    var result = _hiyouService?.countTotal();
    Utility.PrintInfoMessage($"{result:C}");
}

void countSpecificHiyou() {
    if (!IsInputValid(commands, 2))
        return;

    if (int.TryParse(commands[1], out int month)) {
        var result = _hiyouService?.countTotalSpecificMonth(month);
        Utility.PrintInfoMessage($"{result:C}");
            
    } else {
        Utility.PrintErrorMessage("Wrong command. Please try again.");
        Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
        return;
    }
}

void DisplayAllCommands() {
    var commandRisuto = _hiyouService?.getAllCommands();
    int count = 1;

    if (commandRisuto != null)
        foreach (var i in commandRisuto) {
            Utility.PrintHelpMessage(count + ". " + i);
            count++;
        }
}

#region helper methods

static void ConfigureServices(IServiceCollection i) {
    i.AddSingleton<IHiyouServices, HiyouServices>();
} 

static void DisplayWelcomeMessage() {
    Utility.PrintInfoMessage("Hi, welcome to Hiyou Tracker.");
    Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
}

static bool IsInputValid(List<string> commands, int requiredQuantity) {
    bool ifValid = true;

    if (requiredQuantity == 1)
        if (commands.Count != 1)
            ifValid = false;

    if (requiredQuantity == 2)
        if (commands.Count != 2 || string.IsNullOrEmpty(commands[1]))
            ifValid = false;

    if (requiredQuantity == 3)
        if (commands.Count != 3 || string.IsNullOrEmpty(commands[1]) || string.IsNullOrEmpty(commands[2]))
            ifValid = false;

    if (!ifValid) {
        Utility.PrintErrorMessage("Wrong command. Please try again.");
        Utility.PrintInfoMessage(@"Type \help\ to know the set of commands");
        return false;
    }

    return true;
} 

static void DisplayHiyouAsTable(List<Hiyou> risuto) {
    int colWidth1 = 5, colWidth2 = 15, colWidth3 = 15, colWidth4 = 5;
    if (risuto != null && risuto.Count > 0) {
        Console.WriteLine("\n{0,-" + colWidth1 + "} {1,-" + colWidth2 + "} {2,-" + colWidth3 + "} {3,-" + colWidth4 + "}",
            "ID", "Date", "Description", "Amount" + "\n");

        foreach (var i in risuto) {
            Console.WriteLine("{0,-" + colWidth1 + "} {1,-" + colWidth2 + "} {2,-" + colWidth3 + "} {3,-" + colWidth4 + "}", 
                i.ID, i.Date.ToString("dd-MM-yyyy"), i.Description, i.Amount.ToString("C"));
            
        }

    } else {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n No Hiyou exists! \n");
        Console.ResetColor();

        Console.WriteLine("{0,-" + colWidth1 + "} {1,-" + colWidth2 + "} {2,-" + colWidth3 + "} {3,-" + colWidth4 + "}",
           "ID", "Date", "Description", "Amount");
    }
}

#endregion