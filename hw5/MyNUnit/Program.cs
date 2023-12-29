﻿namespace MyNUnit;

using System.Reflection;

/// <summary>
/// Tests runner. Runs tests from the specified assembly.
/// </summary>
internal class Program
{
    /// <summary>
    /// Runs tests from the specified assembly.
    /// </summary>
    /// <param name="args">The path to the assembly.</param>
    /// <returns>The number of failed tests.</returns>
    public static async Task<int> Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: MyNUnit <assembly>");
            return 1;
        }

        var assembly = Assembly.LoadFrom(args[0]);

        var results = await TestRunner.RunAssemblyTests(assembly);

        int errors = 0;

        foreach (var result in results)
        {
            if (result is TestResult.Ok ok)
            {
                Console.WriteLine($"{ok.className}.{ok.methodName} passed in {ok.elapsed}");
            }

            if (result is TestResult.Error error)
            {
                Console.WriteLine(
                    $"{error.className}.{error.methodName} failed in {error.elapsed}: {error.exception}"
                );
                errors++;
            }

            if (result is TestResult.Ignored ignored)
            {
                Console.WriteLine(
                    $"{ignored.className}.{ignored.methodName} ignored: {ignored.reason}"
                );
            }
        }

        return errors;
    }
}
