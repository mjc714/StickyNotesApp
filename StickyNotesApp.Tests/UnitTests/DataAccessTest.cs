using StickyNotesApp.Data;
using StickyNotesApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;
using System;

namespace StickyNotesApp.Tests.UnitTests
{
    public class DataAccessTest
    {
        private readonly string user = "test123@abc.com";

        [Fact]
        public async Task IndexAuth_ReturnsCorrectResult()
        {
            using (var db = new ApplicationDbContext(Utilities.Utilities.TestingDbContextOptions()))
            {
                // Arrange
                var expectedTodos = ApplicationDbContext.GetSeedingTodos();
                await db.AddRangeAsync(expectedTodos);
                await db.SaveChangesAsync();

                // Act
                //var result = expectedTodos.Where(note => note.OwnerID == user).ToList();
                var result = await db.GetTodosAsync();

                // Assert
                var actualTodos = Assert.IsAssignableFrom<List<Todo>>(result);
                Assert.Equal(3, actualTodos.Where(n => n.OwnerID == user).Count());
                Assert.Equal(
                    expectedTodos.Select(n => n.OwnerID == user),
                    actualTodos.Select(n => n.OwnerID == user)
                    );
            }
        }

        [Fact]
        public async Task CreateAuth_TodoIsAdded()
        {
            using (var db = new ApplicationDbContext(Utilities.Utilities.TestingDbContextOptions()))
            {
                // Arrange
                var expectedTodo = new Todo() { ID = 13, Title = "Testing543.", OwnerID = "test123@abc.com", Description = "Testing tests.", IsDone = false, ExpireDate = DateTime.Now };

                // Act
                await db.AddAsync(expectedTodo);
                await db.SaveChangesAsync();

                // Assert
                var actualTodo = await db.FindAsync<Todo>(13);
                Assert.Equal(expectedTodo, actualTodo);
                Assert.Equal(user, actualTodo.OwnerID);
            }
        }

        [Fact]
        public void EditAuth_CorrectTodoisEdited()
        {
            using (var db = new ApplicationDbContext(Utilities.Utilities.TestingDbContextOptions()))
            {
                // Arrange
                var expectedTodos = ApplicationDbContext.GetSeedingTodos();
                var todo = expectedTodos.Find(n => n.ID == 1);
                var newTitle = "EditTest.";
                var newDescription = "SuchTestsMuchWow";

                // Act
                var newTodo = new Todo() { ID = todo.ID, Title = newTitle, OwnerID = todo.OwnerID, Description = newDescription, IsDone = todo.IsDone, ExpireDate = todo.ExpireDate };

                // Assert
                Assert.NotEqual(todo.Title, newTodo.Title);
                Assert.NotEqual(todo.Description, newTodo.Description);
                Assert.Equal(todo.OwnerID, newTodo.OwnerID);
            }
        }

        [Fact]
        public async Task DeleteAllAuth_AllCorrectTodosAreDeleted()
        {
            using (var db = new ApplicationDbContext(Utilities.Utilities.TestingDbContextOptions()))
            {
                // Arrange
                var seedTodos = ApplicationDbContext.GetSeedingTodos();
                await db.AddRangeAsync(seedTodos);
                await db.SaveChangesAsync();

                // Act
                await db.DeleteAllTodosAsync();

                // Assert
                Assert.Empty(await db.Todos.AsNoTracking().ToListAsync());
            }

        }

        [Fact]
        public async Task DeleteAuth_CorrectTodoisDeleted()
        {
            using (var db = new ApplicationDbContext(Utilities.Utilities.TestingDbContextOptions()))
            {
                // Arrange
                var seedTodos = ApplicationDbContext.GetSeedingTodos();
                await db.AddRangeAsync(seedTodos);
                await db.SaveChangesAsync();
                var delID = 3;
                var expectedTodos = seedTodos.Where(todo => todo.ID != delID).ToList();

                // Act
                await db.DeleteTodoAsync(delID, user);

                // Assert
                var actualTodos = await db.Todos.AsNoTracking().ToListAsync();
                Assert.Equal(
                    expectedTodos.Select(note => note.Title),
                    actualTodos.Select(note => note.Title)
                    );
                Assert.Equal(expectedTodos.Count(), actualTodos.Count());
            }
        }

        [Fact]
        public async Task DeleteAuth_IncorrectTodoisNotDeleted()
        {
            using (var db = new ApplicationDbContext(Utilities.Utilities.TestingDbContextOptions()))
            {
                // Arrange
                var expectedTodos = ApplicationDbContext.GetSeedingTodos();
                await db.AddRangeAsync(expectedTodos);
                await db.SaveChangesAsync();
                var delID = -1;

                // Act
                await db.DeleteTodoAsync(delID, user);

                // Assert
                var actualTodos = await db.Todos.AsNoTracking().ToListAsync();
                Assert.Equal(
                    expectedTodos.Select(note => note.Title),
                    actualTodos.Select(note => note.Title)
                    );
                Assert.Equal(expectedTodos.Count(), actualTodos.Count());
            }
        }
    }
}
