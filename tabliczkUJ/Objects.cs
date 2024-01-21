using System.Windows.Controls;
using Newtonsoft.Json;

namespace tabliczkUJ
{
  interface IContentObject
  {
    public void Serialize();
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

    public void Serialize()
    {
      JsonConvert.SerializeObject(this);
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

    public void Serialize()
    {
      JsonConvert.SerializeObject(this);
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

    public void Serialize()
    {
      JsonConvert.SerializeObject(this);
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
