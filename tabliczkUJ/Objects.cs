using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows.Media;

namespace tabliczkUJ
{

  class DoorTag
  {
    public LogoObject LogoObject { get; set; } = new();
    public TextObject RoomObject { get; set; } = new();
    public TextObject MembersObject { get; set; } = new();
  }

  class LogoObject
  {
    public ImageSource? Source { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Top { get; set; }
    public double Left { get; set; }
  }

  class TextObject
  {
    public FontFamily? FontFamily { get; set; }
    public double FontSize { get; set; }
    public Brush? Foreground { get; set; }
    public string? FlowDocumentString { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Top { get; set; }
    public double Left { get; set;}
  }
}
