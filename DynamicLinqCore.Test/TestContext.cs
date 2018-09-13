using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.EntityFrameworkCore;

namespace DynamicLinqCore.Test
{
    public class TestEntity1
    {
        public int Id { get; set; }

        public int? NullableInt { get; set; }

        public string Name { get; set; }

        public TestEntity2 ChildEntity { get; set; }
    }

    public class TestEntity2
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<TestEntity3> Children { get; set; }
    }

    public class TestEntity3
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class TestContext : DbContext
    {
        private static bool isSeeded = false;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("TestDatabase");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public static void EnsureSeed()
        {
            if (isSeeded)
            {
                return;
            }

            SeedDatabase();
            isSeeded = true;
        }

        private static void SeedDatabase()
        {
            var dbContext = new TestContext();

            var childchild1 = new TestEntity3()
            {
                Id = 10,
                Name = "ChildChild1"
            };

            var childchild2 = new TestEntity3()
            {
                Id = 11,
                Name = "ChildChild2"
            };

            var childchild3 = new TestEntity3()
            {
                Id = 12,
                Name = "ChildChild3"
            };

            var child1 = new TestEntity2()
            {
                Id = 5,
                Name = "Child1",
                Children = new List<TestEntity3>
                {
                    childchild1,
                    childchild2
                }
            };

            var child2 = new TestEntity2()
            {
                Id = 6,
                Name = "Child2",
                Children = new List<TestEntity3>
                {
                    childchild3
                }
            };

            dbContext.Add(
                new TestEntity1()
                {
                    Id = 1,
                    NullableInt = 2,
                    Name = "John",
                    ChildEntity = child1
                });

            dbContext.Add(
                new TestEntity1()
                {
                    Id = 2,
                    Name = "James",
                    ChildEntity = child2
                });

            dbContext.Add(
                new TestEntity1()
                {
                    Id = 3,
                    NullableInt = 4,
                    Name = "Lea"
                });

            dbContext.SaveChanges();
        }

        public DbSet<TestEntity1> TestEntity1s { get; set; }
        public DbSet<TestEntity1> TestEntity2s { get; set; }
        public DbSet<TestEntity1> TestEntity3s { get; set; }
    }

    public static class DbSetHelper
    {
        public static IQueryable AsQueryable<T>(this DbSet<T> dbSet)
            where T: class
        {
            return dbSet;
        }

    }
}
