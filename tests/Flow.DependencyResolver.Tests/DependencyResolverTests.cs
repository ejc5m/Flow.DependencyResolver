using Flow.DependencyResolver.Diagnostics;

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

            var fails = results.Failures.FailuresByKey["B"];
            Assert.That(fails[0].Reason is MissingDependencyFailure<string>);
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
            Assert.That(results.Ordered.Count is 0);

            Assert.That(results.Failures.FailuresByKey.Keys.Count() is 2);

            var aFailures = results.Failures.FailuresByKey["A"];
            var bFailures = results.Failures.FailuresByKey["B"];

            Assert.That(aFailures.Count is 1);
            Assert.That(bFailures.Count is 1);

            {
                Assert.That(aFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["A", "B"]));
            }
            {
                Assert.That(bFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["A", "B"]));
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
            Assert.That(results.Ordered.Count is 0);

            Assert.That(results.Failures.FailuresByKey.Keys.Count() is 4);

            var aFailures = results.Failures.FailuresByKey["A"];
            var bFailures = results.Failures.FailuresByKey["B"];
            var cFailures = results.Failures.FailuresByKey["C"];
            var dFailures = results.Failures.FailuresByKey["D"];

            //Should be a single CycleReason and not also a DependsOnInvalid
            Assert.That(aFailures.Count is 1);
            Assert.That(bFailures.Count is 1);
            Assert.That(cFailures.Count is 1);
            Assert.That(dFailures.Count is 1);

            {
                Assert.That(aFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["A", "B", "C", "D"]));
            }
            {
                Assert.That(bFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["A", "B", "C", "D"]));
            }
            {
                Assert.That(cFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["A", "B", "C", "D"]));
            }
            {
                Assert.That(dFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["A", "B", "C", "D"]));
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
            Assert.That(results.Ordered.Count is 0);

            Assert.That(results.Failures.FailuresByKey.Keys.Count() is 3);

            var aFailures = results.Failures.FailuresByKey["A"];
            var bFailures = results.Failures.FailuresByKey["B"];
            var cFailures = results.Failures.FailuresByKey["C"];

            Assert.That(aFailures.Count is 1);
            Assert.That(bFailures.Count is 1);
            Assert.That(cFailures.Count is 1);

            {
                Assert.That(aFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["A", "B"]));
            }
            {
                Assert.That(bFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["A", "B"]));
            }
            {
                Assert.That(cFailures[0].Reason is InvalidDependencyFailure<string> invalid && invalid.Dependency is "A");
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

            Assert.That(results.Failures.FailuresByKey.Keys.Count() is 4);

            var aFailures = results.Failures.FailuresByKey["A"];
            var bFailures = results.Failures.FailuresByKey["B"];
            var cFailures = results.Failures.FailuresByKey["C"];
            var dFailures = results.Failures.FailuresByKey["D"];

            Assert.That(aFailures.Count is 1);
            Assert.That(bFailures.Count is 1);
            Assert.That(cFailures.Count is 1);
            Assert.That(dFailures.Count is 1);

            {
                Assert.That(aFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["A", "B"]));
            }
            {
                Assert.That(bFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["A", "B"]));
            }
            {
                Assert.That(cFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["C", "D"]));
            }
            {
                Assert.That(dFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["C", "D"]));
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

            Assert.That(results.Failures.FailuresByKey.Keys.Count() is 4);

            var missingConsumerFailures = results.Failures.FailuresByKey["MissingConsumer"];
            var cycleAFailures = results.Failures.FailuresByKey["CycleA"];
            var cycleBFailures = results.Failures.FailuresByKey["CycleB"];
            var workerFailures = results.Failures.FailuresByKey["Worker"];

            Assert.That(missingConsumerFailures.Count is 1);
            Assert.That(cycleAFailures.Count is 1);
            Assert.That(cycleBFailures.Count is 1);
            Assert.That(workerFailures.Count is 1);

            {
                Assert.That(missingConsumerFailures[0].Reason is MissingDependencyFailure<string> missing && missing.MissingKey is "NotPresent");
            }
            {
                Assert.That(cycleAFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["CycleA", "CycleB"]));
            }
            {
                Assert.That(cycleBFailures[0].Reason is PartOfACycleFailure<string> cycle && cycle.NodesInCycle.SequenceEqual(["CycleA", "CycleB"]));
            }
            {
                Assert.That(workerFailures[0].Reason is InvalidDependencyFailure<string> invalid && invalid.Dependency is "CycleA");
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
            new TestingItem("Item 4", []),
            new TestingItem("Item 4", []),
            new TestingItem("Item 5", [new("Item 4")]),
        ];

        var results = DependencyResolver.Resolve(Items, item => item.Name, item => item.Dependencies);

        Assert.Multiple(() =>
        {
            //Make sure none of them got added
            Assert.That(results.Ordered.Contains("Item 4") is false);

            //There shouldnt be any failures on this key itself, only a global failure
            Assert.That(!results.Failures.FailuresByKey.ContainsKey("Item 4"));

            //Check for that global failure
            Assert.That(results.Failures.GlobalFailures[0].Reason is DuplicateKeyFailure<string> duplicate && duplicate.Duplicate is "Item 4");

            //Make sure item 5 that depends on the duplicate gets a invalid dependency failure and not a missing dependency failure
            Assert.That(results.Failures.FailuresByKey["Item 5"][0].Reason is InvalidDependencyFailure<string> invalid && invalid.Dependency is "Item 4");
        });
    }
}