using Microsoft.Win32;
using System.CodeDom;
using System.Windows;
using System.Windows.Media.Imaging;

namespace tabliczkUJ
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      var openFileDialog = new OpenFileDialog
      {
        Filter = "Image files|*.png;*.jpeg;*.jpg;*.gif|All files|*.*"
      };
      if (openFileDialog.ShowDialog() == true)
      {
        Uri fileUri = new Uri(openFileDialog.FileName);
        logoImage.Source = new BitmapImage(fileUri);
      }
    }
  }
}