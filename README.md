icm.jsondiff
============

C# library to find differences between two JSON objects.

Example
-------

    IEnumerable<Difference> differences = JsonDiff.Diff(json1, json2);

    if (differences.Any())
    {
        Console.WriteLine("JSON is different!");
        foreach (Difference difference in differences)
        {
            Console.WriteLine(difference);
        }
    }
    else
    {
        Console.WriteLine("JSON is equal!");
    }
