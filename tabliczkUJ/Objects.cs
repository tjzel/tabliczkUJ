using Newtonsoft.Json;
using System.Windows.Controls;

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
  }

  class LogoObject : IContentObject
  {
    public Image Image { get; set; }

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
