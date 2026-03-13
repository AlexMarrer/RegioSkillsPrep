using Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database;

public class ApplicationDbContext : DbContext
{
  public virtual DbSet<Exercise> Exercise { get; set; }
  public virtual DbSet<Template> Template { get; set; }
  public virtual DbSet<TemplateExercise> TemplateExercise { get; set; }
  public virtual DbSet<TemplateSet> TemplateSet { get; set; }
  public virtual DbSet<Workout> Workout { get; set; }
  public virtual DbSet<WorkoutExercise> WorkoutExercise { get; set; }
  public virtual DbSet<WorkoutSet> WorkoutSet { get; set; }

  public ApplicationDbContext()
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
  }

  protected override void OnConfiguring(DbContextOptionsBuilder options)
  {
    var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "regioskills.db");
    options.UseSqlite($"Data Source={dbPath}");
  }
}
