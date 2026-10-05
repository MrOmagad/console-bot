using System.Reflection;
using static System.Console;

namespace bot
{
  internal class Program
  {
    private const string ListOfCommands = "/start\n/help\n/info\n/menu\n/clear\n/exit";
    private const string ListOfCommandExtended = "/start\n/help\n/info\n/menu\n/clear\n/echo [текст]\n/addtask\n/showtask\n/removetask\n/exit\n";

    static List<string> tasks = new List<string>();
    static int taskCountLimit;
    static int taskLengthLimit;

    static void Main(string[] args)
    {
      WriteLine($"Привет!\n{ListOfCommands}");

      bool started = false;
      bool isRunning = true;
      string name = "";

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
              name = WelcomeToProgram();
              started = true;
            }
            else
            {
              WriteLine($"{name}, программа уже работает");
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
              ShowHelp(name);
              break;

            case "/info":
              ShowInfo(name);
              break;

            case "/echo":
              EnterText(name, echoText);
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

            case "/showtask":
              ShowTask();
              break;

            case "/removetask":
              RemoveTask();
              break;

            case "/exit":
              ExitFromProgram(name);
              return;

            default:
              WriteLine("Неверно набрана команда.");
              break;
          }

        }
        catch (ArgumentException ex)
        {
          WriteLine($"Ошибка: {ex.Message}");
          ReadKey();
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
    private static string WelcomeToProgram()
    {
      WriteLine("\nКак тебя зовут?");
      string name = ReadLine() ?? "";

      ValidateString(name);

      WriteLine("Введите максимально допустимое количество задач: ");
      string input = ReadLine() ?? "";
      taskCountLimit = ParseAndValidate(input, 1, 100);

      WriteLine("Введите максимально допустимую длину задачи");
      string lenghtTask = ReadLine() ?? "";
      taskLengthLimit = ParseAndValidate(lenghtTask, 1, 100);

      WriteLine($"\nПривет, {name}!\nВыбери команду: \n{ListOfCommandExtended}");
      return name;
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

      if (tasks.Contains(task))
      {
        throw new DuplicateTaskException(task);
      }

      tasks.Add(task);
      WriteLine("Задача добавлена!");
    }
    //ДОБАВЛЕНИЕ ЗАДАЧИ

    //ВЫВОД СПИСКА ЗАДАЧ
    private static bool ShowTask()
    {
      if (tasks.Count == 0)
      {
        WriteLine("У вас нет задач");
        return true;
      }

      WriteLine("Список задач:\n");

      for (int i = 0; i < tasks.Count; i++)
      {
        WriteLine($"{i + 1}. {tasks[i]}");
      }
      return false;
    }
    //ВЫВОД СПИСКА ЗАДАЧ

    //УДАЛЕНИЕ ЗАДАЧИ 
    private static void RemoveTask()
    {
      if (ShowTask())
      {
        WriteLine("Список задач пуст. Удалять нечего");
        return;
      }

      WriteLine("\nВведите номер задачи для удаления: ");
      string input = ReadLine();

      if (!int.TryParse(input, out int number))
      {
        WriteLine("Нужно ввести число.");
        return;
      }

      if (number < 1 || number > tasks.Count)
      {
        WriteLine("Неверный номер задачи.");
        return;
      }

      tasks.RemoveAt(number - 1);
      WriteLine("Задача успешно удалена");
    }
    //УДАЛЕНИЕ ЗАДАЧИ

    public class TaskCountLimitException : Exception
    {
      public TaskCountLimitException(int taskCountLimit) : base($"Превышено максимальное количество задач равное {taskCountLimit}")
      {
      }
    }

    public class TaskLengthLimitException : Exception
    {
      public TaskLengthLimitException(int lengthTask, int taskLengthLimit) : base($"Длина задачи {lengthTask} превышает максимально допустимое значение {taskLengthLimit}")
      {
      }
    }

    public class DuplicateTaskException : Exception
    {
      public DuplicateTaskException(string task) : base($"Задача '{task}' уже существует.")
      {
      }
    }

  }
}