icm.jsondiff
============

C# library to find differences between two JSON objects.

Example
-------

    IEnumerable<Difference> differences = JsonDiff.Diff(json1, json2, "ROOT");

    if (differences.Any())
    {
        Console.WriteLine("JSON is different!");
        foreach (Difference difference in differences)
        {
            Console.WriteLine(difference.Path + ": " + difference.Kind);
            Console.WriteLine(difference.Token1);
            Console.WriteLine(difference.Token2);
        }
	}
	else {
		Console.WriteLine("JSON is equal!");
	}
