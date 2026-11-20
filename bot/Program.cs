using System.Reflection;
using static System.Console;

namespace bot
{
  internal class Program
  {
    private const string ListOfCommands = "/start\n/help\n/info\n/menu\n/clear\n/exit";
    private const string ListOfCommandExtended = "/start\n/help\n/info\n/menu\n/clear\n/echo [текст]\n/addtask\n/showtask\n/showalltasks\n/completetask [ID]\n/removetask\n/exit\n";

    static List<ToDoItem> tasks = new List<ToDoItem>();
    static int taskCountLimit;
    static int taskLengthLimit;
    static ToDoUser? currentUser = null;



    static void Main(string[] args)
    {
      WriteLine($"Привет!\n{ListOfCommands}");

      SetTaskLimits();

      bool started = false;
      bool isRunning = true;

      while (isRunning)
      {
        try
        {
          WriteLine("\nВведите команду:");
          string command = ReadLine() ?? "";

          if (command.Equals("/start"))
          {
            if (!started)
            {
              currentUser = WelcomeToProgram();
              started = true;
            }
            else
            {
              WriteLine($"{currentUser!.TelegramUserName}, программа уже работает");
            }

            continue;
          }

          if (!started)
          {
            WriteLine("Для доступа к другим командам сначала введите '/start'.");
            continue;
          }

          string[] parts = command.Split(' ', 2);
          string commandName = parts[0];
          string echoText = parts.Length > 1 ? parts[1].Trim() : "";

          switch (commandName)
          {
            case "/help":
              ShowHelp(currentUser!.TelegramUserName);
              break;

            case "/info":
              ShowInfo(currentUser!.TelegramUserName);
              break;

            case "/echo":
              EnterText(currentUser!.TelegramUserName, echoText);
              break;

            case "/clear":
              Clear();
              break;

            case "/menu":
              WriteLine($"\nСписок команд: \n{ListOfCommandExtended}");
              break;

            case "/addtask":
              AddTask();
              break;

            case "/showtasks":
              ShowTask();
              break;

            case "/showalltasks":
              ShowAllTasks();
              break;

            case "/completetask":
              if (string.IsNullOrWhiteSpace(echoText))
              {
                WriteLine("Укажите ID задачи. Пример: /completetask 12345678-1234-1234-1234-123456789012");
              }
              else
              {
                CompleteTask(echoText);
              }
              break;

            case "/removetask":
              RemoveTask();
              break;

            case "/exit":
              ExitFromProgram(currentUser!.TelegramUserName);
              return;

            default:
              WriteLine("Неверно набрана команда.");
              break;
          }

        }
        catch (TaskCountLimitException ex)
        {
          Console.WriteLine(ex.Message);
          ReadKey();
        }
        catch (TaskLengthLimitException ex)
        {
          WriteLine(ex.Message);
        }
        catch (DuplicateTaskException ex)
        {
          WriteLine(ex.Message);
        }
        catch (ArgumentException ex)
        {
          WriteLine($"Ошибка: {ex.Message}");
          ReadKey();
        }
        catch (Exception error)
        {
          Console.WriteLine("Произошла непредвиденная ошибка");
          Console.WriteLine($"Type: {error.GetType()}");
          Console.WriteLine($"Message: {error.Message}");
          Console.WriteLine($"StackTrace: {error.StackTrace}");
          Console.WriteLine($"InnerException: {error.InnerException}");
          ReadKey();
        }
      }
    }

    //ПРИВЕТСТВИЕ
    private static ToDoUser WelcomeToProgram()
    {
      WriteLine("\nКак тебя зовут?");
      string name = ReadLine() ?? "";

      ValidateString(name);

      ToDoUser user = new ToDoUser(name);

      WriteLine($"\nПривет, {user.TelegramUserName!}\nТвой ID: {user.UserId}\nДата регистрации: {user.RegisteredAt}\nВыбери команду: {ListOfCommandExtended}");
      return user;
    }
    //ПРИВЕТСТВИЕ

    //ПРОВЕРКИ
    private static int ParseAndValidate(string? str, int min, int max)
    {
      if (!int.TryParse(str, out int value))
      {
        throw new ArgumentException("Значение должно быть числом");
      }
      if (value < min || value > max)
      {
        throw new ArgumentException($"Значение должно быть от {min} до {max}");
      }

      return value;
    }

    private static void ValidateString(string? str)
    {
      if (string.IsNullOrWhiteSpace(str))
      {
        throw new ArgumentException("Строка не может пустой");
      }
    }
    //ПРОВЕРКИ

    //ЛИМИТЫ
    private static void SetTaskLimits()
    {
      WriteLine("Введите максимально допустимое количество задач: ");
      string input = ReadLine() ?? "";
      taskCountLimit = ParseAndValidate(input, 1, 100);

      WriteLine("Введите максимально допустимую длину задачи");
      string lenghtTask = ReadLine() ?? "";
      taskLengthLimit = ParseAndValidate(lenghtTask, 1, 100);
    }
    //ЛИМИТЫ

    //ВЫХОД
    private static void ExitFromProgram(string name)
    {
      WriteLine($"До свидания, {name}");
    }
    //ВЫХОД

    //ВЫВОД ТЕКСТА
    private static void EnterText(string name, string echoText)
    {
      if (string.IsNullOrWhiteSpace(echoText))
      {
        WriteLine($"{name}, после /echo необходимо указать текст.");
        return;
      }

      WriteLine(echoText);
    }
    //ВЫВОД ТЕКСТА

    //СПРАВОЧНАЯ ИНФОРМАЦИЯ О КОМАНДАХ
    private static void ShowHelp(string name)
    {
      WriteLine($"{name}, ниже представлена справочная информация: \n\n" +
        "/start - начать работу\n" +
        "/help - справочная информация\n" +
        "/info - информация о программе\n" +
        "/echo [текс] - вывести текст\n" +
        "/clear - очистить консоль\n" +
        "/addtask - добавление задачи\n" +
        "/showtask - список задач\n" +
        "/showalltask - показать все задачи" +
        "/completetask [ID] - завершить задачу по id" +
        "/removetask - удаление задачи\n" +
        "/exit - выйти из программы\n"
      );
    }
    //СПРАВОЧНАЯ ИНФОРМАЦИЯ О КОМАНДАХ

    //ВЕРСИЯ И ДАТА СОЗДАНИЯ ПРОЕКТА
    private static void ShowInfo(string name)
    {
      Version? version = Assembly.GetExecutingAssembly().GetName().Version;
      WriteLine($"\n{name},\nВерсия программы - {version}\nДата сброки - 14.09.2026");
    }
    //ВЕРСИЯ И ДАТА СОЗДАНИЯ ПРОЕКТА

    //ДОБАВЛЕНИЕ ЗАДАЧИ
    private static void AddTask()
    {
      if (tasks.Count >= taskCountLimit)
      {
        throw new TaskCountLimitException(taskCountLimit);
      }
      WriteLine("Введите описание задачи");
      string task = ReadLine() ?? "";

      if (string.IsNullOrWhiteSpace(task))
      {
        WriteLine("Описание задачи не может быть пустым");
        return;
      }

      if (task.Length > taskLengthLimit)
      {
        throw new TaskLengthLimitException(task.Length, taskLengthLimit);
      }

      for (int i = 0; i < tasks.Count; i++)
      {
        if (tasks[i].Name == task)
        {
          throw new DuplicateTaskException(task);
        }
      }

      ToDoItem newTask = new ToDoItem(currentUser!, task);

      tasks.Add(newTask);
      WriteLine("Задача добавлена!");
      WriteLine($"ID задачи: {newTask.Id}");
    }
    //ДОБАВЛЕНИЕ ЗАДАЧИ

    //ВЫВОД СПИСКА АКТИВНЫХ ЗАДАЧ
    private static bool ShowTask()
    {
      bool hasActiveTasks = false;
      WriteLine("Список активных задач:\n");
      for (int i = 0; i < tasks.Count; i++)
      {
        if (tasks[i].State == ToDoItemState.Active)
        {
          hasActiveTasks = true;

          WriteLine($"Название: {tasks[i].Name}");
          WriteLine($"Дата создания: {tasks[i].CreatedAt}");
          WriteLine($"ID: {tasks[i].Id}\n");
        }
      }
      if (!hasActiveTasks)
      {
        WriteLine("У вас нет активных задач");
      }

      return !hasActiveTasks;
    }
    //ВЫВОД СПИСКА АКТИВНЫХ ЗАДАЧ

    //ВЫВОД СПИСКА ВСЕХ ЗАДАЧ
    private static void ShowAllTasks()
    {
      if (tasks.Count == 0)
      {
        WriteLine("У вас пока нет задач");
        return;
      }

      WriteLine("Полный список задач:\n");
      for (int i = 0; i < tasks.Count; i++)
      {
        WriteLine($"Название: {tasks[i].Name}");
        WriteLine($"Дата создания: {tasks[i].CreatedAt}");
        WriteLine($"ID: {tasks[i].Id}");
        WriteLine($"Статус: {tasks[i].State}\n");
      }
    }
    //ВЫВОД СПИСКА ВСЕХ ЗАДАЧ

    private static void CompleteTask(string taskId)
    {
      if (!Guid.TryParse(taskId, out Guid id))
      {
        WriteLine("Некорректный id задачи");
        return;
      }

      for (int i = 0; i < tasks.Count; i++)
      {
        if (tasks[i].Id == id)
        {
          if (tasks[i].State == ToDoItemState.Completed)
          {
            WriteLine("Эта задача уже завершена");
            return;
          }
          tasks[i].State = ToDoItemState.Completed;
          tasks[i].StateChangedAt = DateTime.UtcNow;

          WriteLine("Задача успешно завершена");
          return;
        }
      }
      WriteLine("Задача с таким ID не найдена");
    }

    //УДАЛЕНИЕ ЗАДАЧИ 
    private static void RemoveTask()
    {
      int activeTaskCount = 0;

      for (int i = 0; i < tasks.Count; i++)
      {
        if (tasks[i].State == ToDoItemState.Active)
        {
          activeTaskCount++;
        }
      }

      if (activeTaskCount == 0)
      {
        WriteLine("Нет активных задач для удаления");
        return;
      }

      WriteLine("Выберите номер активной задачи для удаления:");

      int displayedNumber = 0;

      for (int i = 0; i < tasks.Count; i++)
      {
        if (tasks[i].State == ToDoItemState.Active)
        {
          displayedNumber++;
          WriteLine($"{displayedNumber}. {tasks[i].Name}");
        }
      }

      if (!int.TryParse(ReadLine(), out int number) ||
          number < 1 || number > activeTaskCount)
      {
        WriteLine("Некорректный номер задачи");
        return;
      }

      displayedNumber = 0;

      for (int i = 0; i < tasks.Count; i++)
      {
        if (tasks[i].State == ToDoItemState.Active)
        {
          displayedNumber++;

          if (displayedNumber == number)
          {
            tasks.RemoveAt(i);
            WriteLine("Задача успешно удалена");
            return;
          }
        }
      }
    }
    //УДАЛЕНИЕ ЗАДАЧИ
  }
}