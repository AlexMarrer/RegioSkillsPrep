using Database;
using Database.Entities;
using System.IO;
using System.Windows;

namespace WpfApp;

/// <summary>
/// Interaction logic for App.xaml 
/// </summary>
public partial class App : Application
{
  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    using var applicationDbContext = new ApplicationDbContext();
    applicationDbContext.Database.EnsureCreated();
    SeedExercises(applicationDbContext);
  }

  private void SeedExercises(ApplicationDbContext applicationDbContext)
  {
    if (applicationDbContext.Exercise.Any())
    {
      return;
    }

    var jsonExercises = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "exercises.json"));
    var exercises = System.Text.Json.JsonSerializer.Deserialize<List<Exercise>>(jsonExercises, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    applicationDbContext.Exercise.AddRange(exercises);
    applicationDbContext.SaveChanges();
  }
}
