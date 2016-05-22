# Thor.NET

[<img src="https://travis-ci.org/joncloud/thor_net.svg" />](https://travis-ci.org/joncloud/thor_net/)

## Description
Thor.NET is a port of the ruby [Thor][] framework for building self-documenting command line utilities.

[Thor]: http://whatisthor.com/

## Licensing
Released under the MIT License.  See the [LICENSE][] file for further details.

[license]: LICENSE.md

## Installation
In the Package Manager Console execute

```powershell
Install-Package ThorNet
```

Or update `project.json` to include a dependency on

```json
"ThorNet": "0.4.0"
```

## Usage

### Getting Started

Get started by deriving the console application's `Program` class from `Thor`.  Afterward simply add public methods to be called from the command line.

```csharp
class Program : Thor {

    [Desc("Hello NAME", "say hello to NAME")]
    public void Hello(string name) {
        Console.WriteLine($"Hello {name}");
    }
}
```

In the static `Main` method, call `Start<Program>(args)` to parse the arguments and start the program.

```csharp
static int Main(string[] args) {
    return Start<Program>(args);
}
```

By default running the program with no arguments will call the `help` method.  This prints out all of the public instance methods in the `Program` class:

```powershell
PS C:\MyCLI> dotnet MyCLI.dll
  dotnet MyCLI.dll Hello NAME     # say hello to NAME
  dotnet MyCLI.dll help [COMMAND] # Describe available commands or one specific command
```

If you run the program with the hello argument, then it will call the method:

```powershell
PS C:\MyCLI> dotnet MyCLI.dll Hello Jonathan
Hello Jonathan
```

If you try to run it without the `name` argument, then it will print an error message:

```powershell
PS C:\MyCLI> dotnet MyCLI.dll Hello
"Hello" was called incorrectly. Call as "Hello NAME"
  NAME # Missing parameter
```

Use C#'s optional parameters to indicate whether a parameter is required:

```csharp
class Program : Thor {

    [Desc("Hello NAME", "say hello to NAME")]
    public void Hello(string name, string from = null) {
        if (from != null) { Console.WriteLine($"From: {from}"); }
        Console.WriteLine($"Hello {name}");
    }
}
```

And here is how to call the program:

```powershell
PS C:\MyCLI> dotnet MyCLI.dll Hello Jonathan
Hello Jonathan
PS C:\MyCLI> dotnet MyCLI.dll Hello Jonathan Thor
From: Thor
Hello Jonathan
```

### Exit Codes
Thor.NET will control the exit code for the program.  If everything is successful, then it will return 0.  If an invalid command is provided, or missing parameters, then it will return 1.

```powershell
PS C:\MyCLI> dotnet MyCLI.dll Hello
"Hello" was called incorrectly. Call as "Hello NAME"
  NAME # Missing parameter
PS C:\MyCLI> echo $LASTEXITCODE
1
```

You can control the exit code by returning an `int` or `Task<int>` from your method:

```csharp
class Program : Thor {
    [Desc("Exit CODE", "stops the process with the specified exit code")]
    public int Exit(int code) => code;
}
```

And here is what it looks like:

```powershell
PS C:\MyCLI> dotnet MyCLI.dll Exit 100
PS C:\MyCLI> echo $LASTEXITCODE
100
```

### Long Description

Not currently implemented.

### Options and Flags

Use the `Option` attribute in order to specify options and flags:

```csharp
class Program : Thor {

    [Desc("Hello NAME", "say hello to NAME")]
    [Option("from", "f", "who the message is from")]
    public void Hello(string name) {
        string from = Option("from");
        if (from != null) { Console.WriteLine($"From: {from}"); }
        Console.WriteLine($"Hello {name}");
    }
}
```

Here is how to specify options from the command line:

```powershell
PS C:\MyCLI> dotnet MyCLI.dll Hello Jonathan --from=Thor
From: Thor
Hello Jonathan
PS C:\MyCLI> dotnet MyCLI.dll Hello Jonathan -fThor
From: Thor
Hello Jonathan
```

For flags, specify the `Flag` property, and then check `Flag` for a boolean result:

```csharp
class Program : Thor {

    [Desc("Hello NAME", "say hello to NAME")]
    [Option("from", "f", "who the message is from")]
    [Option("yell", "y", "yells the message", Flag = true)]
    public void Hello(string name) {
        StringBuilder output = new StringBuilder();

        string from = Option("from");
        if (from != null) { output.AppendLine($"From: {from}"); }
        output.AppendLine($"Hello {name}");

        Console.Write(Flag("yell") ? output.ToString().ToUpper() : output.ToString());
    }
}
```

Flags are just options without a value:

```powershell
PS C:\MyCLI> dotnet MyCLI.dll Hello Jonathan --yell --from="Thor"
FROM: THOR
HELLO JONATHAN
PS C:\MyCLI> dotnet MyCLI.dll Hello Jonathan -y --from="Thor"
FROM: THOR
HELLO JONATHAN
```

You can also specify additional properties on `Option`.

* DefaultValue - The default value of the option if none is specified from the command line.
* Flag - Indicates when parsing arguments that no value should be expected.

### Class Options

If you need to use options across multiple commands, then declare `Option` on the class level.  It is used the same way that it is used on a method, but the values become available in any method on the class.

```csharp
[Option("verbose", "v", "prints debugging information", Flag = true)]
public class Program : Thor {
    [Desc("Goodbye", "say goodbye to the world")]
    public void Goodbye() {
        bool verbose = Flag("verbose");

        if (verbose) { Console.WriteLine("> saying goodbye"); }
        Console.WriteLine("Goodbye World");
        if (verbose) { Console.WriteLine("> done saying goodbye"); }
    }

    [Desc("Hello NAME", "say hello to NAME")]
    [Option("from", "f", "who the message is from")]
    [Option("repeat", "r", "repeats the message")]
    [Option("yell", "y", "yells the message", Flag = true)]
    public void Hello(string name)
    {
        bool verbose = Flag("verbose");
        StringBuilder output = new StringBuilder();

        if (verbose) { Console.WriteLine("> saying hello"); }

        string from = Option("from");
        if (from != null) { output.AppendLine($"From: {from}"); }
        output.AppendLine($"Hello {name}");

        int repeats = Option("repeat", defaultValue: () => 1);
        Console.Write(Flag("yell") ? output.ToString().ToUpper() : output.ToString());

        if (verbose) { Console.WriteLine("> done saying hello"); }
    }
}
```

### Type Safety

Arguments can be any type that is convertable using `Convert.ChangeType` or it is an `Enum`:

```csharp
class Program : Thor {

    [Desc("Count TO", "count up to TO")]
    public void Count(int to) {
        for (int i = 1; i <= to; i++) {
            Console.WriteLine(i);
        }
    }
}
```

And the command line arguments look the same:

```powershell
PS C:\MyCLI> dotnet MyCLI.dll Count 1
1
PS C:\MyCLI> dotnet MyCLI.dll Count 2
1
2
```

But if the arguments cannot be parsed:

```powershell
PS C:\MyCLI> dotnet MyCLI.dll Count foo
"Count" was called incorrectly. Call as "count TO"
  TO # Invalid format
```

Options also support types too:

```csharp
class Program : Thor {

    [Desc("Hello NAME", "say hello to NAME")]
    [Option("from", "f", "who the message is from")]
    [Option("repeat", "r", "repeats the message")]
    [Option("yell", "y", "yells the message", Flag = true)]
    public void Hello(string name) {
        StringBuilder output = new StringBuilder();

        string from = Option("from");
        if (from != null) { output.AppendLine($"From: {from}"); }
        output.AppendLine($"Hello {name}");

        int repeats = Option("repeat", defaultValue: () => 1);
        while (--repeats >= 0) {
            Console.Write(Flag("yell") ? output.ToString().ToUpper() : output.ToString());
        }
    }
}
```

Again, the options are the same:

```powershell
PS C:\MyCLI> dotnet MyCLI.dll Hello Jonathan --yell --from="Thor" --repeat=1
FROM: THOR
HELLO JONATHAN
PS C:\MyCLI> dotnet MyCLI.dll Hello Jonathan --yell --from="Thor" --repeat=2
FROM: THOR
HELLO JONATHAN
FROM: THOR
HELLO JONATHAN
```

Options have a couple of extra arguments:

* convert - A delegate that will convert the `string` value into the requested type.
* defaultValue - A delegate that provides the default value if no option is provided.

### Subcommands

Subcommands allow you to organize your code better as your program grows.  Simply create a new class that derives from `Thor`, and then in the main `Program` create a constructor that calls `Subcommand<T>`.

```csharp
class Messages : Thor {
    public void Add(string message) {
        // ...
    }

    public void List() {
    }

    public void Remove(int id) {
        // ...
    }
}

class Program : Thor {
    public Program() {
        Subcommand<Messages>("messages");
    }

    [Desc("Hello NAME", "say hello to NAME")]
    [Option("from", "f", "who the message is from")]
    [Option("repeat", "r", "repeats the message")]
    [Option("yell", "y", "yells the message", Flag = true)]
    public void Hello(string name) {
        // ...
    }
}
```

Help is updated too:

```powershell
PS C:\MyCLI> dotnet MyCLI.dll
  dotnet MyCLI.dll Hello NAME      # say hello to NAME
  dotnet MyCLI.dll help [COMMAND]  # Describe available commands or one specific command
  dotnet MyCLI.dll messages Add    # ...
  dotnet MyCLI.dll messages List   # ...
  dotnet MyCLI.dll messages Remove # ...
```

### Task async/await
Using async/await Tasks are supported too:

```csharp
class Program : Thor {
    [Desc("Delay TIME", "delays in milliseconds")]
    public async Task Delay(int time) {
        bool verbose = Flag("verbose");

        if (verbose) { Console.WriteLine($"> delaying {time}ms"); }
        await Task.Delay(time);
        if (verbose) { Console.WriteLine($"> done delaying {time}ms"); }
    }
}
```

### And More
Check out the [sample][] project for more.

[sample]: src/ThorNet.Sample
