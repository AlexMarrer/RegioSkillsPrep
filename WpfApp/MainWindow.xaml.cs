using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp.Views;

namespace WpfApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
  private UserControl _previousView;

  public MainWindow()
  {
    InitializeComponent();
    NavigateTo(new WorkoutsView());
  }

  private void Exercises_Click(object sender, RoutedEventArgs e)
  {
    NavigateTo(new ExercisesView());
  }

  private void Templates_Click(object sender, RoutedEventArgs e)
  {
    NavigateTo(new TemplatesView());
  }

  private void Back_Click(object sender, RoutedEventArgs e)
  {
    if (_previousView is not null)
    {
      NavigateTo(_previousView);
    }
  }

  public void NavigateTo(UserControl view)
  {
    var isMainView = view is WorkoutsView;

    if (isMainView)
    {
      _previousView = null;
      NavButtons.Visibility = Visibility.Visible;
      BackButton.Visibility = Visibility.Collapsed;
    }
    else
    {
      _previousView = MainContent.Content as UserControl;
      NavButtons.Visibility = Visibility.Collapsed;
      BackButton.Visibility = Visibility.Visible;
    }

    switch (view)
    {
      case WorkoutsView:
      case ExercisesView:
      case TemplatesView:
        _previousView = new WorkoutsView();
        break;
      default:
        break;
    }

    MainContent.Content = view;
  }
}