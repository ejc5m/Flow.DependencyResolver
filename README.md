# Flow.DependencyResolver

A simple dependency resolver for dotnet.

It resolves dependencies by removing duplicates, detecting cycles, propogating failures, and topologically sorting the dependencies.

## Quick Example

### There are two main ways to use this:

Option 1 - Creating the items separately beforehand and resolving them after:
```csharp
using Flow.DependencyResolver;

//An example of a possible type you could use
public record TestingItem(string Name, Dependency<string>[] Dependencies);

List<TestingItem> items =
[
    new TestingItem("Item 1", []),
    new TestingItem("Item 2", [new Dependency<string>("Item 4").Optional()]),
    new TestingItem("Item 3", [new("Item 5")]),
    new TestingItem("Item 4", [new("Item 9")]),
    new TestingItem("Item 4", []),
    new TestingItem("Item 5", []),

    new TestingItem("Item 6", [new("Item 7")]),
    new TestingItem("Item 7", [new("Item 6")]),
];

//Now you can resolve the dependencies using one of the following methods using the Key string of our item as the identifier, but you can use anything 

//Option 1:
DependencyResolutionResult<string> results = DependencyResolver.Resolve(items, i => i.Name, i => i.Dependencies);
//Option 2:
DependencyResolutionResult<string> results = DependencyResolver.From(items).WithKey(i => i.Name).WithDependencies(i => i.Dependencies).Resolve();
```

### Option 2 - Using a fluent style API to create a dependency graph and resolve all at once:
```csharp
DependencyResolutionResult<string> results = DependencyGraph.Create()
    .Add("Item 1")
    .Add("Item 2").OptionallyDependsOn("Item 4")
    .Add("Item 3").DependsOn("Item 5")
    .Add("Item 4").DependsOn("Item 9")
    .Add("Item 4")
    .Add("Item 5")
    .Add("Item 6").DependsOn("Item 7")
    .Add("Item 7").DependsOn("Item 6")
    .Resolve();
```
# Results
The results of the dependency resolution can be accessed from `results.Ordered`.

From the example above, the items inside would be 
```csharp
["Item 1", "Item 5", "Item 3"]
```
The reasons why these are the only items in the output can be found in the Failures section.
# Failures
All failures encountered along the way will be added to a Failure Collection you can access after resolution from `results.FailureCollection`.

All the failures have a default message you can use, so
```csharp
foreach (var failure in results.Failures.EnumerateFailures())
    Console.WriteLine(failure);
```
will print out
```
Duplicate key 'Item 4'.
Item 4: Missing required dependency 'Item 9'.
Item 6: Part of cycle 'Item 6 -> Item 7 -> Item 6'.
Item 7: Part of cycle 'Item 6 -> Item 7 -> Item 6'.
Item 2: Required dependency 'Item 4' is not usable because it failed for one or more reasons.
```
But you can also make your own message using failure.Reason, and if the failure is of type KeyedFailure\<TKey> you can cast and get failure.Key from it.

You can also use the `results.Failures.FailuresByKey` dictionary to manually enumerate failures on all nodes or to get failures for a specific node.

And you can use `results.Failures.GlobalFailures` to manually enumerate all failures not attached to a specific node (Duplicate key failures).