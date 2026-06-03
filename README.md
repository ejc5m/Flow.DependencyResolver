# Flow.DependencyResolver

A simple dependency resolver for dotnet

It resolves dependencies by removing duplicates, detecting cycles, propogating failures, and topologically sorting the dependencies

## Quick Example

### There are two main ways to use this:
- Create the items separately beforehand and resolve them later
- Use a fluent builder style API to create a dependency graph and resolve all at once

### For the first option you can create the items you want to use:
```csharp
using Flow.DependencyResolver;

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
```
### And resolve them with this
```csharp
DependencyResolutionResult<string> results = DependencyResolver.Resolve(items, i => i.Name, i => i.Dependencies);
```
### or
```csharp
DependencyResolutionResult<string> results = DependencyResolver.From(items).WithKey(i => i.Name).WithDependencies(i => i.Dependencies).Resolve();
```

### And for the second option you can use this
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
The results of the dependency resolution can be accessed from
```csharp
results.Ordered
```
From the example above, the items inside would be 
```csharp
["Item 1", "Item 5", "Item 3"]
```
The reasons why these are the only items in the output can be found in the Failures section
# Failures
All failures encountered along the way will be added to a Failure Collection you can access after resolution from
```csharp
results.FailureCollection
```

All the failures have a default message you can use, so
```csharp
foreach (var failure in results.Failures.EnumerateFailures())
    Console.WriteLine(failure);
```
will print
```csharp
Duplicate key 'Item 4'.
Item 4: Missing dependency 'Item 9'.
Item 6: Part of cycle 'Item 6 -> Item 7 -> Item 6'.
Item 7: Part of cycle 'Item 6 -> Item 7 -> Item 6'.
Item 2: Required dependency 'Item 4' is invalid.
```
But you can also make your own message using failure.Reason, and if the failure is of type KeyedFailure\<TKey> you can cast and get failure.Key from it

You can also use the `results.Failures.FailuresByKey` dictionary to manually enumerate failures on all nodes or to get failures for a specific node.

And you can use `results.Failures.GlobalFailures` to manually enumerate all failures not attached to a specific node (Duplicate key failures)