using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Big_data
{
    class ApplicationContext: DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = localhost; Database = moviebase; Trusted_Connection=True; "); 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>()
               .HasOne(m => m.Director)
               .WithMany(d => d.Movies);
            modelBuilder.Entity<Actor>()
                .HasMany(a => a.Movies)
                .WithMany(m => m.Actors)
                .UsingEntity(j => j.ToTable("Connection1"));
            modelBuilder.Entity<Tag>()
                .HasMany(t => t.Movies)
                .WithMany(m => m.Tags)
                .UsingEntity(j => j.ToTable("Connection2"));
        }


    }
}
