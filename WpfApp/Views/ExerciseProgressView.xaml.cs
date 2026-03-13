using Database;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Windows.Controls;
using WpfApp.DTO;

namespace WpfApp.Views;

/// <summary>
/// Interaktionslogik für ExerciseProgressView.xaml
/// </summary>
public partial class ExerciseProgressView : UserControl
{
  private Guid _exerciseId;

  public ExerciseProgressView(Guid exerciseId, string exerciseName)
  {
    InitializeComponent();
    _exerciseId = exerciseId;
    PageTitle.Text = $"Progress of {exerciseName}";
    LoadProgress();
  }

  private void LoadProgress()
  {
    var allRecords = GetProgressRecords();

    if (!allRecords.Any())
    {
      NoDataMessage.Visibility = System.Windows.Visibility.Visible;
      DataPanel.Visibility = System.Windows.Visibility.Collapsed;
      ExpectedPanel.Visibility = System.Windows.Visibility.Collapsed;
      return;
    }

    ProgressGrid.ItemsSource = allRecords;

    var nextExpected = CalculateNextExpected(allRecords);
    ExpectedValue.Text = $"{Math.Round(nextExpected, 1)}kg";

    SetupChart(allRecords, nextExpected);
  }

  private List<ExerciseProgressDto> GetProgressRecords()
  {
    using var ctx = new ApplicationDbContext();

    return ctx.WorkoutExercise
        .Include(we => we.Workout)
        .Include(we => we.WorkoutSets)
        .Where(we => we.ExerciseId == _exerciseId)
        .Select(we => new ExerciseProgressDto
        {
          Date = we.Workout.StartDateTime,
          TotalVolume = we.WorkoutSets.Sum(s => s.Weight * s.Repetitions)
        })
        .OrderBy(r => r.Date)
        .ToList();
  }

  private decimal CalculateNextExpected(List<ExerciseProgressDto> records)
  {
    if (records.Count < 2)
    {
      return records.Last().TotalVolume;
    }

    var increases = new List<decimal>();
    for (int i = 1; i < records.Count; i++)
    {
      increases.Add(records[i].TotalVolume - records[i - 1].TotalVolume);
    }

    var averageIncrease = increases.Average();
    return records.Last().TotalVolume + averageIncrease;
  }

  private void SetupChart(List<ExerciseProgressDto> allRecords, decimal nextExpected)
  {
    var threeMonthsAgo = DateTime.Now.AddMonths(-3);
    var recentRecords = allRecords
        .Where(r => r.Date >= threeMonthsAgo)
        .ToList();

    var values = new ChartValues<double>(
        recentRecords.Select(r => (double)r.TotalVolume));
    values.Add((double)nextExpected);

    var labels = recentRecords
        .Select(r => r.Date.ToString("dd.MM.yyyy"))
        .ToList();
    labels.Add("Expected");

    ProgressChart.Series = new SeriesCollection
        {
            new LineSeries
            {
                Title = "Total Volume",
                Values = values,
                PointGeometrySize = 8
            }
        };

    ProgressChart.AxisX.Clear();
    ProgressChart.AxisX.Add(new Axis
    {
      Labels = labels,
      LabelsRotation = 45
    });
  }
}
