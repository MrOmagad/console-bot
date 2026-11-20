using System;
using System.Collections.Generic;
using System.Text;

namespace bot
{
  public enum ToDoItemState
  {
    Active,
    Completed
  }
  public class ToDoItem
  {
    public Guid Id {  get; }
    public ToDoUser User { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; }
    public ToDoItemState State { get; set; }
    public DateTime? StateChangedAt { get; set; }

    public ToDoItem(ToDoUser user, string name)
    {
      Id = Guid.NewGuid();
      CreatedAt = DateTime.UtcNow;
      State = ToDoItemState.Active;
      User = user;
      Name = name;
    }
  }
}
