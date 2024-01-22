using Newtonsoft.Json;
using System.Windows.Controls;
using static tabliczkUJ.MainWindow;

namespace tabliczkUJ
{
  interface IContentObject
  {
    public string Serialize();
    public void SetPosition(int x, int y);
    public void SetSize(int x, int y);
  }

  class DoorTag
  {
    public string Name { get; set; }
    public ICollection<IContentObject> ContentObjects;
    public LogoObject LogoObject { get; set; } = new();

    //[JsonConverter(typeof(RoomNumberObjectConverter))]
    public RoomNumberObject RoomNumberObject { get; set; } = new();
    public RoomMembersObject RoomMembersObject { get; set; } = new();
  }

  class LogoObject : IContentObject
  {
    public Image Image { get; set; }
    public object Value { get; set; }

    public string Serialize()
    {
      return JsonConvert.SerializeObject(this);
    }

    public void SetPosition(int x, int y)
    {
      throw new NotImplementedException();
    }

    public void SetSize(int x, int y)
    {
      throw new NotImplementedException();
    }
  }

  class RoomNumberObject : IContentObject
  {
    public string RoomNumberText { get; set; }

    public object Value { get; set; }

    public string Serialize()
    {
      return JsonConvert.SerializeObject(this);
    }

    public void SetPosition(int x, int y)
    {
      throw new NotImplementedException();
    }

    public void SetSize(int x, int y)
    {
      throw new NotImplementedException();
    }
  }

  class RoomMembersObject : IContentObject
  {
    public string RoomMembersText { get; set; }

    public object Value { get; set; }


    public string Serialize()
    {
      return JsonConvert.SerializeObject(this);
    }

    public void SetPosition(int x, int y)
    {
      throw new NotImplementedException();
    }

    public void SetSize(int x, int y)
    {
      throw new NotImplementedException();
    }
  }
}
