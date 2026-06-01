using Flow.DependencyResolver.Diagnostics;
using NUnit.Framework.Interfaces;

namespace Flow.DependencyResolver.Tests;

public class Tests
{
    [Test]
    public void SimpleDependencyChainTest()
    {
        List<TestingItem> Items = [];
        Items.Add(new TestingItem() { Name = "A" });
        Items.Add(new TestingItem() { Name = "B", Dependencies = [new Dependency<string>("A")] });
        Items.Add(new TestingItem() { Name = "C", Dependencies = [new Dependency<string>("B")] });
        Items.Add(new TestingItem() { Name = "D", Dependencies = [new Dependency<string>("C")] });

        var results = DependencyResolver.Resolve(Items, item => item.Name, item => item.Dependencies);

        Assert.That(results.Ordered.SequenceEqual(["A", "B", "C", "D"]));
    }

    [Test]
    public void DiamondDependencyTest()
    {
        List<TestingItem> Items = [];
        Items.Add(new TestingItem() { Name = "A" });
        Items.Add(new TestingItem()
        {
            Name = "B",
            Dependencies = [new Dependency<string>("A")],
        });
        Items.Add(new TestingItem()
        {
            Name = "C",
            Dependencies = [new Dependency<string>("A")],
        });
        Items.Add(new TestingItem()
        {
            Name = "D",
            Dependencies =
            [
                new Dependency<string>("B"),
                new Dependency<string>("C"),
            ],
        });

        var results = DependencyResolver.Resolve(Items, item => item.Name, item => item.Dependencies);

        Assert.That(results.Ordered.SequenceEqual(["A", "B", "C", "D"]));
    }

    [Test]
    public void MissingRequiredDependencyTest()
    {
        List<TestingItem> Items = [];
        Items.Add(new TestingItem() { Name = "A" });
        Items.Add(new TestingItem()
        {
            Name = "B",
            Dependencies = [new Dependency<string>("MissingItem")],
        });

        var results = DependencyResolver.Resolve(Items, item => item.Name, item => item.Dependencies);

        Assert.Multiple(() =>
        {
            Assert.That(results.Ordered.SequenceEqual(["A"]));

            var fails = results.Failures.GetFailureReasons(results.Failures.FailedKeys[0]);
            Assert.That(fails[0] is MissingDependencyFailure<string>);
        });
    }

    [Test]
    public void MissingOptionalDependencyTest()
    {
        List<TestingItem> Items = [];
        Items.Add(new TestingItem() { Name = "A" });
        Items.Add(new TestingItem()
        {
            Name = "B",
            Dependencies = [new Dependency<string>("MissingItem").Optional()],
        });

        var results = DependencyResolver.Resolve(Items, item => item.Name, item => item.Dependencies);
        Assert.That(results.Ordered.SequenceEqual(["A", "B"]));
    }

    [Test]
    public void SimpleCycleTest()
    {
        List<TestingItem> Items = [];
        Items.Add(new TestingItem()
        {
            Name = "A",
            Dependencies = [new Dependency<string>("B")],
        });
        Items.Add(new TestingItem()
        {
            Name = "B",
            Dependencies = [new Dependency<string>("A")],
        });

        var results = DependencyResolver.Resolve(Items, item => item.Name, item => item.Dependencies);

        Assert.Multiple(() =>
        {
            Assert.That(results.Ordered.SequenceEqual([]));

            Assert.That(results.Failures.FailedKeys.Count is 2);
            foreach (var key in results.Failures.FailedKeys)
            {
                var failReasons = results.Failures.GetFailureReasons(key);
                //Should be a single CycleReason and not also a DependsOnInvalid
                Assert.That(failReasons.Count is 1);

                Assert.That(failReasons[0] is PartOfACycleFailure<string> cycle && cycle.Cycle.NodesInCycle.SequenceEqual(["A", "B"]));
            }
        });
    }

    [Test]
    public void AdvancedCycleTest()
    {
        List<TestingItem> Items = [];
        Items.Add(new TestingItem()
        {
            Name = "A",
            Dependencies = [new Dependency<string>("B")],
        });
        Items.Add(new TestingItem()
        {
            Name = "B",
            Dependencies = [new Dependency<string>("C")],
        });
        Items.Add(new TestingItem()
        {
            Name = "C",
            Dependencies = [new Dependency<string>("D")],
        });
        Items.Add(new TestingItem()
        {
            Name = "D",
            Dependencies = [new Dependency<string>("A")],
        });

        var results = DependencyResolver.Resolve(Items, item => item.Name, item => item.Dependencies);

        Assert.Multiple(() =>
        {
            Assert.That(results.Ordered.SequenceEqual([]));

            Assert.That(results.Failures.FailedKeys.Count is 4);
            foreach (var key in results.Failures.FailedKeys)
            {
                var failReasons = results.Failures.GetFailureReasons(key);
                //Should be a single CycleReason and not also a DependsOnInvalid
                Assert.That(failReasons.Count is 1);

                Assert.That(failReasons[0] is PartOfACycleFailure<string> cycle && cycle.Cycle.NodesInCycle.SequenceEqual(["A", "B", "C", "D"]));
            }
        });
    }

    [Test]
    public void ItemDependingOnACycleTest()
    {
        List<TestingItem> Items = [];
        Items.Add(new TestingItem()
        {
            Name = "A",
            Dependencies = [new Dependency<string>("B")],
        });
        Items.Add(new TestingItem()
        {
            Name = "B",
            Dependencies = [new Dependency<string>("A")],
        });

        Items.Add(new TestingItem()
        {
            Name = "C",
            Dependencies = [new Dependency<string>("A")],
        });

        var results = DependencyResolver.Resolve(Items, item => item.Name, item => item.Dependencies);

        Assert.Multiple(() =>
        {
            Assert.That(results.Ordered.SequenceEqual([]));

            Assert.That(results.Failures.FailedKeys.Count is 3);
            foreach (var key in results.Failures.FailedKeys)
            {
                var failReasons = results.Failures.GetFailureReasons(key);
                //Each should have 1 failure
                Assert.That(failReasons.Count is 1);

                //Part of cycle
                if (key is "A" or "B")
                {
                    Assert.That(failReasons[0] is PartOfACycleFailure<string> cycle && cycle.Cycle.NodesInCycle.SequenceEqual(["A", "B"]));
                }
                //Depends on the cycle
                else if (key is "C")
                {
                    Assert.That(failReasons[0] is InvalidDependencyFailure<string> invalid && invalid.Dependency is "A");
                }
            }
        });
    }

    [Test]
    public void MultipleCycles()
    {
        List<TestingItem> Items = [];
        Items.Add(new TestingItem()
        {
            Name = "A",
            Dependencies = [new Dependency<string>("B")],
        });
        Items.Add(new TestingItem()
        {
            Name = "B",
            Dependencies = [new Dependency<string>("A")],
        });

        Items.Add(new TestingItem()
        {
            Name = "C",
            Dependencies = [new Dependency<string>("D")],
        });
        Items.Add(new TestingItem()
        {
            Name = "D",
            Dependencies = [new Dependency<string>("C")],
        });

        Items.Add(new TestingItem() { Name = "E" });

        var results = DependencyResolver.Resolve(Items, item => item.Name, item => item.Dependencies);

        Assert.Multiple(() =>
        {
            Assert.That(results.Ordered.SequenceEqual(["E"]));

            Assert.That(results.Failures.FailedKeys.Count is 4);
            foreach (var key in results.Failures.FailedKeys)
            {
                var failReasons = results.Failures.GetFailureReasons(key);
                //Each should have 1 failure
                Assert.That(failReasons.Count is 1);

                //Part of cycle 1
                if (key is "A" or "B")
                {
                    Assert.That(failReasons[0] is PartOfACycleFailure<string> cycle && cycle.Cycle.NodesInCycle.SequenceEqual(["A", "B"]));
                }
                //Part of cycle 2
                else if (key is "C" or "D")
                {
                    Assert.That(failReasons[0] is PartOfACycleFailure<string> cycle && cycle.Cycle.NodesInCycle.SequenceEqual(["C", "D"]));
                }
            }
        });
    }

    [Test]
    public void MixedTest()
    {
        List<TestingItem> Items = [];
        Items.Add(new TestingItem() { Name = "Config" });
        Items.Add(new TestingItem()
        {
            Name = "Database",
            Dependencies = [new Dependency<string>("Config")],
        });
        Items.Add(new TestingItem()
        {
            Name = "Cache",
            Dependencies = [new Dependency<string>("Config")],
        });
        Items.Add(new TestingItem()
        {
            Name = "Api",
            Dependencies =
            [
                new Dependency<string>("Database"),
                new Dependency<string>("Cache")
            ],
        });
        Items.Add(new TestingItem()
        {
            Name = "CycleA",
            Dependencies = [new Dependency<string>("CycleB")],
        });
        Items.Add(new TestingItem()
        {
            Name = "CycleB",
            Dependencies = [new Dependency<string>("CycleA")],
        });
        Items.Add(new TestingItem()
        {
            Name = "Worker",
            Dependencies =
            [
                new Dependency<string>("Api"),
                new Dependency<string>("CycleA")
            ],
        });
        Items.Add(new TestingItem()
        {
            Name = "MissingConsumer",
            Dependencies = [new Dependency<string>("NotPresent")],
        });
        Items.Add(new TestingItem()
        {
            Name = "OptionalConsumer",
            Dependencies = [new Dependency<string>("NotPresent").Optional()],
        });

        var results = DependencyResolver.Resolve(Items, item => item.Name, item => item.Dependencies);

        Assert.Multiple(() =>
        {
            Assert.That(results.Ordered.SequenceEqual(["Config", "OptionalConsumer", "Database", "Cache", "Api"]));

            Assert.That(results.Failures.FailedKeys.Count is 4);
            foreach (var key in results.Failures.FailedKeys)
            {
                var failReasons = results.Failures.GetFailureReasons(key);
                //Each should have 1 failure
                Assert.That(failReasons.Count is 1);

                if (key is "MissingConsumer")
                {
                    Assert.That(failReasons[0] is MissingDependencyFailure<string> missing && missing.MissingKey is "NotPresent");
                }
                else if (key is "CycleA" or "CycleB")
                {
                    Assert.That(failReasons[0] is PartOfACycleFailure<string> cycle && cycle.Cycle.NodesInCycle.SequenceEqual(["CycleA", "CycleB"]));
                }
                else if (key is "Worker")
                {
                    Assert.That(failReasons[0] is InvalidDependencyFailure<string> invalid && invalid.Dependency is "CycleA");
                }
            }
        });
    }

    [Test]
    public void DuplicateKeys()
    {
        List<TestingItem> Items =
        [
            new TestingItem("Item 1", []),
            new TestingItem("Item 2", [new Dependency<string>("Item 4").Optional()]),
            new TestingItem("Item 3", [new("Item 5")]),
            new TestingItem("Item 4", [new("Item 9")]),
            new TestingItem("Item 4", []),
            new TestingItem("Item 5", []),
        ];

        var results = DependencyResolver.Resolve(Items, item => item.Name, item => item.Dependencies);

        Assert.Multiple(() =>
        {
            //Make sure the duplicate didn't get added
            Assert.That(results.Ordered.Contains("Item 4") is false);

            //Also make sure that the 1st instance was added and failed
            Assert.That(results.Failures.GetFailureReasons("Item 4")[0] is MissingDependencyFailure<string>);

            Assert.That(results.Failures.GetGlobalFailures()[0] is DuplicateKeyFailure<string> duplicate && duplicate.Duplicate is "Item 4");
        });
    }
}