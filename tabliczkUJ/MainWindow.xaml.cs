using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace tabliczkUJ
{
  public partial class MainWindow : Window
  {

    private DoorTag doorTag = new();

    private bool isResizing;

    private Point mousePosition;

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
        var saveFileDialog = new SaveFileDialog
        {
          Filter = "Pliki PNG (*.png)|*.png",
          Title = "Zapisz PNG",
          FileName = "tabliczka.png"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
          var renderBitmap = new RenderTargetBitmap(
              (int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
          renderBitmap.Render(canvas);

          var pngEncoder = new PngBitmapEncoder();
          pngEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

          using (var stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
          {
            pngEncoder.Save(stream);
          }

          MessageBox.Show($"PNG zapisano pomyślnie w {saveFileDialog.FileName}");
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Błąd podczas zapisu PNG: {ex.Message}");
      }
    }

    private void OnLoadLogoClick(object sender, RoutedEventArgs e)
    {
      var openFileDialog = new OpenFileDialog
      {
        Filter = "Pliki obrazów|*.png;*.jpeg;*.jpg;*.gif|Wszystkie pliki|*.*"
      };
      if (openFileDialog.ShowDialog() == true)
      {
        Uri fileUri = new(openFileDialog.FileName);
        Image image = GetObjectFromThumb<Image>(imageThumb, "logoImage");
        image.Source = new BitmapImage(fileUri);
      }
    }

    private void OnThumbDragStarted(object sender, DragStartedEventArgs e)
    {
      var startPoint = Mouse.GetPosition(this);
      var thumb = sender as Thumb;
      var left = Canvas.GetLeft(thumb);
      var top = Canvas.GetTop(thumb);
      isResizing = new[] { Math.Abs(left - startPoint.X), Math.Abs(left + thumb.Width - startPoint.X), Math.Abs(top - startPoint.Y), Math.Abs(top + thumb.Height - startPoint.Y) }.Min() < 10;
      mousePosition = Mouse.GetPosition(thumb);
    }


    private void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
    {
      var thumb = sender as Thumb;
      var newMousePosition = Mouse.GetPosition(thumb);
      if (thumb != null)
      {


        if (isResizing)
        {
          var xDiff = newMousePosition.X - mousePosition.X;
          var yDiff = newMousePosition.Y - mousePosition.Y;
          thumb.Width += xDiff;
          thumb.Height += yDiff;
        }
        else
        {
          double left = Canvas.GetLeft(thumb) + e.HorizontalChange;
          double top = Canvas.GetTop(thumb) + e.VerticalChange;

          Canvas.SetLeft(thumb, left);
          Canvas.SetTop(thumb, top);
        }
      }

      mousePosition = newMousePosition;
    }

    private void OnEnableDragClick(object sender, RoutedEventArgs e)
    {
      var button = sender as Button;
      button.Content = "Edytuj zawartość";

      imageThumb.DragStarted += OnThumbDragStarted;
      imageThumb.DragDelta += OnThumbDragDelta;

      roomThumb.DragStarted += OnThumbDragStarted;
      roomThumb.DragDelta += OnThumbDragDelta;

      namesThumb.DragStarted += OnThumbDragStarted;
      namesThumb.DragDelta += OnThumbDragDelta;

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
      button.Content = "Edytuj elementy";

      imageThumb.DragStarted -= OnThumbDragStarted;
      imageThumb.DragDelta -= OnThumbDragDelta;

      roomThumb.DragStarted -= OnThumbDragStarted;
      roomThumb.DragDelta -= OnThumbDragDelta;

      namesThumb.DragStarted -= OnThumbDragStarted;
      namesThumb.DragDelta -= OnThumbDragDelta;

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
        var roomTextBox = GetObjectFromThumb<RichTextBox>(roomThumb, "roomTextBox");
        doorTag.RoomObject.Width = roomThumb.Width;
        doorTag.RoomObject.Height = roomThumb.Height;
        doorTag.RoomObject.Top = Canvas.GetTop(roomThumb);
        doorTag.RoomObject.Left = Canvas.GetLeft(roomThumb);
        doorTag.RoomObject.FontFamily = roomTextBox.FontFamily;
        doorTag.RoomObject.Foreground = roomTextBox.Foreground;
        doorTag.RoomObject.FontSize = roomTextBox.FontSize;
        doorTag.RoomObject.FlowDocumentString = XamlWriter.Save(roomTextBox.Document);

        var namesTextBox = GetObjectFromThumb<RichTextBox>(namesThumb, "namesTextBox");
        doorTag.MembersObject.Width = namesThumb.Width;
        doorTag.MembersObject.Height = namesThumb.Height;
        doorTag.MembersObject.Top = Canvas.GetTop(namesThumb);
        doorTag.MembersObject.Left = Canvas.GetLeft(namesThumb);
        doorTag.MembersObject.FontFamily = namesTextBox.FontFamily;
        doorTag.MembersObject.Foreground = namesTextBox.Foreground;
        doorTag.MembersObject.FontSize = namesTextBox.FontSize;
        doorTag.MembersObject.FlowDocumentString = XamlWriter.Save(namesTextBox.Document);

        var image = GetObjectFromThumb<Image>(imageThumb, "logoImage");
        doorTag.LogoObject.Source = image.Source;
        doorTag.LogoObject.Width = imageThumb.Width;
        doorTag.LogoObject.Height = imageThumb.Height;
        doorTag.LogoObject.Top = Canvas.GetTop(imageThumb);
        doorTag.LogoObject.Left = Canvas.GetLeft(imageThumb);

        var saveFileDialog = new SaveFileDialog
        {
          Filter = "Pliki JSON (*.json)|*.png",
          Title = "Zapisz tabliczkę",
          FileName = "tabliczka.json"
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

          MessageBox.Show($"Tabliczka zapisana w {saveFileDialog.FileName}");
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Błąd zapisu tabliczki: {ex.Message}");
      }
    }

    private void OnLoadClick(object sender, RoutedEventArgs e)
    {
      try
      {
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
          Filter = "Pliki JSON (*.json)|*.json",
          Title = "Otwórz tabliczkę"
        };

        if (openFileDialog.ShowDialog() == true)
        {
          string jsonContent = File.ReadAllText(openFileDialog.FileName);

          DoorTag loadedDoorTag = JsonConvert.DeserializeObject<DoorTag>(jsonContent);
          var roomObject = loadedDoorTag.RoomObject;
          var membersObject = loadedDoorTag.MembersObject;
          var logoObject = loadedDoorTag.LogoObject;

          var roomTextBox = GetObjectFromThumb<RichTextBox>(roomThumb, "roomTextBox");
          roomTextBox.FontFamily = roomObject.FontFamily;
          roomTextBox.Foreground = roomObject.Foreground;
          roomTextBox.FontSize = roomObject.FontSize;
          roomTextBox.Document = XamlReader.Parse(roomObject.FlowDocumentString) as FlowDocument;
          roomThumb.Width = roomObject.Width;
          roomThumb.Height = roomObject.Height;
          Canvas.SetTop(roomThumb, roomObject.Top);
          Canvas.SetLeft(roomThumb, roomObject.Left);

          var namesTextBox = GetObjectFromThumb<RichTextBox>(namesThumb, "namesTextBox");
          namesTextBox.FontFamily = membersObject.FontFamily;
          namesTextBox.Foreground = membersObject.Foreground;
          namesTextBox.FontSize = membersObject.FontSize;
          namesTextBox.Document = XamlReader.Parse(membersObject.FlowDocumentString) as FlowDocument;
          namesThumb.Width = membersObject.Width;
          namesThumb.Height = membersObject.Height;
          Canvas.SetTop(namesThumb, membersObject.Top);
          Canvas.SetLeft(namesThumb, membersObject.Left);

          var logoImage = GetObjectFromThumb<Image>(imageThumb, "logoImage");
          logoImage.Source = logoObject.Source;
          imageThumb.Width = logoObject.Width;
          imageThumb.Height = logoObject.Height;
          Canvas.SetTop(imageThumb, logoObject.Top);
          Canvas.SetLeft(imageThumb, logoObject.Left);

          MessageBox.Show($"Tabliczka wczytana pomyślnie.");
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Błąd odczytu tabliczki: {ex.Message}");
      }
    }

    private void OnMouseEnterThumb(object sender, MouseEventArgs e)
    {
      var thumb = sender as Thumb;
      var border = GetObjectFromThumb<Border>(thumb, "border");
      border.BorderThickness = new Thickness(2);
    }

    private void OnMouseLeaveThumb(object sender, MouseEventArgs e)
    {
      var thumb = sender as Thumb;
      var border = GetObjectFromThumb<Border>(thumb, "border");
      border.BorderThickness = new Thickness(0);
    }

    private void OnFontFamilySelectedRoomText(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        var content = (e.AddedItems[0] as ComboBoxItem).Content as string;
        var roomTextBox = GetObjectFromThumb<RichTextBox>(roomThumb, "roomTextBox");
        roomTextBox.FontFamily = new FontFamily(content);
      }
      catch { }
    }

    private void OnFontSizeSelectedRoomText(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        var content = (e.AddedItems[0] as ComboBoxItem).Content as string;
        var roomTextBox = GetObjectFromThumb<RichTextBox>(roomThumb, "roomTextBox");
        roomTextBox.FontSize = double.Parse(content);
      }
      catch { }
    }

    private void OnForegroundSelectedRoomText(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        var content = (e.AddedItems[0] as ComboBoxItem).Tag as string;
        var roomTextBox = GetObjectFromThumb<RichTextBox>(roomThumb, "roomTextBox");
        roomTextBox.Foreground = (Brush)new BrushConverter().ConvertFromString(content);
      }
      catch { }
    }

    private void OnFontFamilySelectedNamesText(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        var content = (e.AddedItems[0] as ComboBoxItem).Content as string;
        var namesTextBox = GetObjectFromThumb<RichTextBox>(namesThumb, "namesTextBox");
        namesTextBox.FontFamily = new FontFamily(content);
      }
      catch { }
    }

    private void OnFontSizeSelectedNamesText(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        var content = (e.AddedItems[0] as ComboBoxItem).Content as string;
        var namesTextBox = GetObjectFromThumb<RichTextBox>(namesThumb, "namesTextBox");
        namesTextBox.FontSize = double.Parse(content);
      }
      catch { }
    }

    private void OnForegroundSelectedNamesText(object sender, SelectionChangedEventArgs e)
    {
      try
      {
        var content = (e.AddedItems[0] as ComboBoxItem).Tag as string;
        var namesTextBox = GetObjectFromThumb<RichTextBox>(namesThumb, "namesTextBox");
        namesTextBox.Foreground = (Brush)new BrushConverter().ConvertFromString(content);
      }
      catch { }
    }
  }
}