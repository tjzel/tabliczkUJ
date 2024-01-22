using Microsoft.Win32;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;
using Newtonsoft.Json.Linq;

namespace tabliczkUJ
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    private DoorTag doorTag = new();

    public MainWindow()
    {
      InitializeComponent();
    }

    private T GetObjectFromThumb<T>(Thumb thumb, string name) where T : class
    {

      return thumb.Template.FindName(name, thumb) as T;
    }

    private void ExportCanvasToPng(Canvas canvas)
    {
      try
      {
        // Create a SaveFileDialog to choose the PNG file location
        var saveFileDialog = new SaveFileDialog
        {
          Filter = "PNG files (*.png)|*.png",
          Title = "Save PNG Image",
          FileName = "output.png"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
          // Create a RenderTargetBitmap from the Canvas
          var renderBitmap = new RenderTargetBitmap(
              (int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
          renderBitmap.Render(canvas);

          // Encode the RenderTargetBitmap as a PNG image
          var pngEncoder = new PngBitmapEncoder();
          pngEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

          // Save the PNG file
          using (var stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
          {
            pngEncoder.Save(stream);
          }

          MessageBox.Show($"PNG image saved successfully to {saveFileDialog.FileName}");
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Error exporting to PNG: {ex.Message}");
      }
    }

    private void OnLoadLogoClick(object sender, RoutedEventArgs e)
    {
      var openFileDialog = new OpenFileDialog
      {
        Filter = "Image files|*.png;*.jpeg;*.jpg;*.gif|All files|*.*"
      };
      if (openFileDialog.ShowDialog() == true)
      {
        Uri fileUri = new(openFileDialog.FileName);
        Image image = GetObjectFromThumb<Image>(imageThumb, "logoImage");
        image.Source = new BitmapImage(fileUri);
      }
    }

    private void OnImageThumbDrag(object sender, DragDeltaEventArgs e)
    {
      var thumb = sender as Thumb;

      double left = Canvas.GetLeft(thumb) + e.HorizontalChange;
      double top = Canvas.GetTop(thumb) + e.VerticalChange;

      Canvas.SetLeft(thumb, left);
      Canvas.SetTop(thumb, top);

      Trace.WriteLine($"left: {e.HorizontalChange}, top: {e.VerticalChange}");
    }

    private void OnRoomThumbDrag(object sender, DragDeltaEventArgs e)
    {
      var thumb = sender as Thumb;

      double left = Canvas.GetLeft(thumb) + e.HorizontalChange;
      double top = Canvas.GetTop(thumb) + e.VerticalChange;

      Canvas.SetLeft(thumb, left);
      Canvas.SetTop(thumb, top);

      Trace.WriteLine($"left: {e.HorizontalChange}, top: {e.VerticalChange}");
    }

    private void OnNamesThumbDrag(object sender, DragDeltaEventArgs e)
    {
      var thumb = sender as Thumb;

      double left = Canvas.GetLeft(thumb) + e.HorizontalChange;
      double top = Canvas.GetTop(thumb) + e.VerticalChange;

      Canvas.SetLeft(thumb, left);
      Canvas.SetTop(thumb, top);

      Trace.WriteLine($"left: {e.HorizontalChange}, top: {e.VerticalChange}");
    }

    private void OnEnableDragClick(object sender, RoutedEventArgs e)
    {
      var button = sender as Button;
      button.Content = "Edytuj zawartość";

      imageThumb.DragDelta += OnImageThumbDrag;
      roomThumb.DragDelta += OnRoomThumbDrag;
      namesThumb.DragDelta += OnNamesThumbDrag;

      var roomText = GetObjectFromThumb<RichTextBox>(roomThumb, "roomTextBox");
      var namesText = GetObjectFromThumb<RichTextBox>(namesThumb, "namesTextBox");
      roomText.IsEnabled = false;
      namesText.IsEnabled = false;

      button.Click -= OnEnableDragClick;
      button.Click += OnEnableEditClick;
    }

    private void OnEnableEditClick(object sender, RoutedEventArgs e)
    {
      var button = sender as Button;
      button.Content = "Przesuwaj elementy";

      imageThumb.DragDelta -= OnImageThumbDrag;
      roomThumb.DragDelta -= OnRoomThumbDrag;
      namesThumb.DragDelta -= OnNamesThumbDrag;

      var roomText = GetObjectFromThumb<RichTextBox>(roomThumb, "roomTextBox");
      var namesText = GetObjectFromThumb<RichTextBox>(namesThumb, "namesTextBox");
      roomText.IsEnabled = true;
      namesText.IsEnabled = true;

      button.Click -= OnEnableEditClick;
      button.Click += OnEnableDragClick;
    }

    private void OnGenerateClick(object sender, RoutedEventArgs e)
    {
      ExportCanvasToPng(doorTagCanvas);
    }


  private void OnSaveClick(object sender, RoutedEventArgs e)
    {
      try
      {
        doorTag.RoomNumberObject.Value = XamlWriter.Save(GetObjectFromThumb<RichTextBox>(roomThumb, "roomTextBox"));
        doorTag.LogoObject.Value = XamlWriter.Save(GetObjectFromThumb<Image>(imageThumb, "logoImage"));
        doorTag.RoomMembersObject.Value = XamlWriter.Save(GetObjectFromThumb<RichTextBox>(namesThumb, "namesTextBox"));
        // Create a SaveFileDialog to choose the PNG file location
        var saveFileDialog = new SaveFileDialog
        {
          Filter = "JSON files (*.json)|*.png",
          Title = "Save JSON",
          FileName = "output.json"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
          JsonSerializer serializer = new()
          {
            NullValueHandling = NullValueHandling.Include
          };

          using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName))
          using (JsonWriter jsonWriter = new JsonTextWriter(streamWriter))
          {
            serializer.Serialize(jsonWriter, doorTag);
          }

          MessageBox.Show($"JSON saved successfully to {saveFileDialog.FileName}");
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Error exporting to JSON: {ex.Message}");
      }
    }
  }
}