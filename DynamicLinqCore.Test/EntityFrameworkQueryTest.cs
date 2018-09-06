using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Xunit;

using DynamicLinqCore.EntityFramework;

namespace DynamicLinqCore.Test
{
    public class EntityFrameworkQueryTest
    {
        [Fact]
        public void DbContextWhereQueryWorks()
        {
            TestContext.EnsureSeed();

            var dbContext = new TestContext();

            var results = dbContext.TestEntity1s
                                   .AsQueryable()
                                   .Where("x => x.Name == \"John\"")
                                   .ToArrayAsync()
                                   .Result
                                   .Cast<TestEntity1>()
                                   .ToArray();

            results.Length.Should().Be(1);
            results[0].Id.Should().Be(1);
            results[0].Name.Should().Be("John");
            results[0]
                .ChildEntity.Should()
                .BeNull();
        }

        [Fact]
        public void DbContextWhereQueryWithIncludeWorks()
        {
            TestContext.EnsureSeed();

            var dbContext = new TestContext();

            var results = dbContext.TestEntity1s
                                   .AsQueryable()
                                   .Where("x => x.Name == \"John\"")
                                   .Include("ChildEntity")
                                   .ToArrayAsync()
                                   .Result
                                   .Cast<TestEntity1>()
                                   .ToArray();

            results.Length.Should().Be(1);
            results[0].Id.Should().Be(1);
            results[0].Name.Should().Be("John");
            results[0].ChildEntity.Id.Should().Be(5);
            results[0].ChildEntity.Name.Should().Be("Child1");
            results[0].ChildEntity.Children.Should().BeNull();
        }

        [Fact]
        public void DbContextWhereQueryWithThenIncludeWorks()
        {
            TestContext.EnsureSeed();

            var dbContext = new TestContext();

            var results = dbContext.TestEntity1s
                                   .AsQueryable()
                                   .Where("x => x.Name == \"John\"")
                                   .Include("ChildEntity.Children")
                                   .ToArrayAsync()
                                   .Result
                                   .Cast<TestEntity1>()
                                   .ToArray();

            results.Length.Should().Be(1);
            results[0].Id.Should().Be(1);
            results[0].Name.Should().Be("John");
            results[0].ChildEntity.Id.Should().Be(5);
            results[0].ChildEntity.Name.Should().Be("Child1");
            results[0].ChildEntity.Children.Count.Should().Be(2);
            results[0].ChildEntity.Children.First().Id.Should().Be(10);
            results[0].ChildEntity.Children.First().Name.Should().Be("ChildChild1");
            results[0].ChildEntity.Children.Last().Id.Should().Be(11);
            results[0].ChildEntity.Children.Last().Name.Should().Be("ChildChild2");
        }

        [Fact]
        public void DbContextWhereOrderByQueryWorks()
        {
            TestContext.EnsureSeed();

            var dbContext = new TestContext();

            var results = dbContext.TestEntity1s
                                   .AsQueryable()
                                   .Where("x => x.Name.StartsWith(\"J\")")
                                   .OrderBy("x => x.Name", typeof(string))
                                   .ToArrayAsync()
                                   .Result
                                   .Cast<TestEntity1>()
                                   .ToArray();

            results.Length.Should().Be(3);

            results[0].Id.Should().Be(2);
            results[0].Name.Should().Be("James");
            results[0].ChildEntity.Should().BeNull();

            results[1].Id.Should().Be(4);
            results[1].Name.Should().Be("James");
            results[1].ChildEntity.Should().BeNull();

            results[2].Id.Should().Be(1);
            results[2].Name.Should().Be("John");
            results[2].ChildEntity.Should().BeNull();
        }

        [Fact]
        public void DbContextWhereOrderByThenByQueryWorks()
        {
            TestContext.EnsureSeed();

            var dbContext = new TestContext();

            var results = dbContext.TestEntity1s
                                   .AsQueryable()
                                   .OrderBy("x => x.Name", typeof(string))
                                   .ThenByDescending("x => x.Id", typeof(int))
                                   .ToArrayAsync()
                                   .Result
                                   .Cast<TestEntity1>()
                                   .ToArray();

            results.Length.Should().Be(4);

            results[0].Id.Should().Be(4);
            results[0].Name.Should().Be("James");
            results[0].ChildEntity.Should().BeNull();

            results[1].Id.Should().Be(2);
            results[1].Name.Should().Be("James");
            results[1].ChildEntity.Should().BeNull();

            results[2].Id.Should().Be(1);
            results[2].Name.Should().Be("John");
            results[2].ChildEntity.Should().BeNull();

            results[3].Id.Should().Be(3);
            results[3].Name.Should().Be("Lea");
            results[3].ChildEntity.Should().BeNull();
        }

        [Fact]
        public void DbContextWhereOrderByDescendingQueryWorks()
        {
            TestContext.EnsureSeed();

            var dbContext = new TestContext();

            var results = dbContext.TestEntity1s.AsQueryable()
                                   .Where("x => x.Name.StartsWith(\"J\")")
                                   .OrderByDescending("x => x.Id", typeof(int))
                                   .ToArrayAsync()
                                   .Result
                                   .Cast<TestEntity1>()
                                   .ToArray();

            results.Length.Should().Be(3);

            results[0].Id.Should().Be(4);
            results[0].Name.Should().Be("James");
            results[0].ChildEntity.Should().BeNull();

            results[1].Id.Should().Be(2);
            results[1].Name.Should().Be("James");
            results[1].ChildEntity.Should().BeNull();

            results[2].Id.Should().Be(1);
            results[2].Name.Should().Be("John");
            results[2].ChildEntity.Should().BeNull();
        }

        [Fact]
        public void DbContextWhereOrderByDescendingThenByQueryWorks()
        {
            TestContext.EnsureSeed();

            var dbContext = new TestContext();

            var results = dbContext.TestEntity1s.AsQueryable()
                                   .Where("x => x.Name.StartsWith(\"J\")")
                                   .OrderByDescending("x => x.Name", typeof(string))
                                   .ThenBy("x => x.ChildEntity.Name", typeof(string))
                                   .ToArrayAsync()
                                   .Result
                                   .Cast<TestEntity1>()
                                   .ToArray();

            results.Length.Should().Be(3);
            
            results[0].Id.Should().Be(1);
            results[0].Name.Should().Be("John");
            results[0].ChildEntity.Should().BeNull();

            results[1].Id.Should().Be(4);
            results[1].Name.Should().Be("James");
            results[1].ChildEntity.Should().BeNull();

            results[2].Id.Should().Be(2);
            results[2].Name.Should().Be("James");
            results[2].ChildEntity.Should().BeNull();
        }

        [Fact]
        public void DbContextWhereSkipAndTakeQueryWorks()
        {
            TestContext.EnsureSeed();

            var dbContext = new TestContext();

            var results = dbContext.TestEntity1s
                                   .AsQueryable()
                                   .Where("x => x.Name.Length > 1")
                                   .Skip(1)
                                   .Take(1)
                                   .ToArrayAsync()
                                   .Result
                                   .Cast<TestEntity1>()
                                   .ToArray();

            results.Length.Should().Be(1);

            results.First().Id.Should().Be(2);
            results.First().Name.Should().Be("James");
            results.First().ChildEntity.Should().BeNull();
        }
    }
}
