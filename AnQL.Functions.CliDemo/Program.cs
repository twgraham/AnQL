
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using AnQL.Core;
using AnQL.Functions;
using AnQL.Functions.CliDemo;
using AnQL.Functions.Time;

[assembly:ExcludeFromCodeCoverage(Justification = "Example project")]

Console.WriteLine("AnQL CLI Demo");
Console.WriteLine();
ShowHelp();

var anqlParser = new AnQLBuilder().ForFunctions<Car>()
    .AddNaturalTime()
    .RegisterAllProperties()
    .Build();

var jsonOptions = new JsonSerializerOptions { WriteIndented = true };

while (true)
{
    Console.Write("AnQL> ");
    var line = Console.ReadLine();

    if (line == null)
        continue;

    if (line.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
        break;

    if (line.Equals("help", StringComparison.InvariantCultureIgnoreCase))
    {
        ShowHelp();
        continue;
    }

    var stopwatch = Stopwatch.StartNew();
    var filter = line.Equals("show", StringComparison.InvariantCultureIgnoreCase) ? _ => true : anqlParser.Parse(line);
    stopwatch.Stop();
    
    var filteredCars = StaticData.Cars.Where(filter).ToList();
    var json = JsonSerializer.Serialize(filteredCars, jsonOptions);

    Console.WriteLine("Result:");
    Console.WriteLine(json);
    Console.WriteLine("Query parse time: {0}ms", stopwatch.ElapsedMilliseconds);
}

void ShowHelp()
{
    Console.WriteLine("[query]  - AnQL Query to filter test data");
    Console.WriteLine("show     - Shows all of test data (no filter)");
    Console.WriteLine("exit     - Quit the program");
    Console.WriteLine();
}
