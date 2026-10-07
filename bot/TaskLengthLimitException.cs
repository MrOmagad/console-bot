namespace bot
{
  public class TaskLengthLimitException : Exception
  {
    public TaskLengthLimitException(int lengthTask, int taskLengthLimit) : base($"Длина задачи {lengthTask} превышает максимально допустимое значение {taskLengthLimit}")
    {
    }
  }
}
