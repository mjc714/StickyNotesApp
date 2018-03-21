using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StickyNotesApp.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace StickyNotesApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Todo> Todos { get; set; }

        public async virtual Task<List<Todo>> GetTodosAsync()
        {
            return await Todos.ToListAsync();
        }

        public async virtual Task DeleteAllTodosAsync()
        {
            foreach (Todo todo in Todos)
            {
                Todos.Remove(todo);
            }
            await SaveChangesAsync();
        }

        public async virtual Task DeleteTodoAsync(int id, string user)
        {
            var todo = await Todos.FindAsync(id);

            if (todo != null && todo.OwnerID == user)
            {
                Todos.Remove(todo);
                await SaveChangesAsync();
            }
        }

        public static List<Todo> GetSeedingTodos()
        {
            return new List<Todo>()
            {
                new Todo(){ID = 1, Title = "Testing.", OwnerID = "test123@abc.com", Description = "Testing tests.", IsDone = false, ExpireDate = DateTime.Now},
                new Todo(){ID = 2, Title = "Testing2.", OwnerID = "test123@abc.com", Description = "Testing tests." , IsDone = false, ExpireDate = new DateTime(2018, 3, 21)},
                new Todo(){ID = 3, Title = "Testing3.", OwnerID = "test123@abc.com", Description = "Testing tests." , IsDone = false, ExpireDate = new DateTime(2018, 3, 22)},
                new Todo(){ID = 4, Title = "Testing4.", OwnerID = "abc111@abc.com", Description = "Testing tests." , IsDone = false, ExpireDate = new DateTime(2018, 3, 18)},
                new Todo(){ID = 5, Title = "Testing5.", OwnerID = "abc111@abc.com", Description = "Testing tests.", IsDone = false, ExpireDate = DateTime.Now}
            };
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<Todo>().ToTable("Todo");
        }
    }
}
