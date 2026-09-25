using System.Reflection;
using System.Xml.Linq;
using static System.Console;

namespace bot
{
  internal class Program
  {
    private const string ListOfCommands = "/start\n/help\n/info\n/menu\n/clear\n/exit";
    private const string ListOfCommandExtended = "/start\n/help\n/info\n/menu\n/clear\n/echo [текст]\n/addtask\n/showtask\n/removetask\n/exit\n";

    static List<string> tasks = new List<string>();

    static void Main(string[] args)
    {
      WriteLine($"Привет!\n{ListOfCommands}");

      bool started = false;
      string name = "";

      while (true)
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

        if (command.StartsWith("/echo"))
        {
          string echoText = command[6..];

          if (string.IsNullOrWhiteSpace(echoText))
          {
            WriteLine($"{name}, после /echo необходимо указать текст.");
          }
          else
          {
            WriteLine($"{echoText}");
          }
          continue;
        }
        if (command.Equals("/exit"))
        {
          WriteLine($"До свидания, {name}");
          break;
        }
        SelectCommand(command, name);

      }
    }

    //ПРИВЕТСТВИЕ
    private static string WelcomeToProgram()
    {
      WriteLine("\nКак тебя зовут?");
      string name = Console.ReadLine() ?? "";
      WriteLine($"\nПривет, {name}!\nВыбери команду: \n{ListOfCommandExtended}");
      return name;
    }
    //ПРИВЕТСТВИЕ

    //ВЫБОР КОМАНДЫ
    private static void SelectCommand(string command, string name)
    {
      switch (command)
      {
        case "/help":
          ShowHelp(name);
          break;

        case "/info":
          ShowInfo(name);
          break;

        case "/echo":
          WriteLine($"{name}, используйте команду в формате: /echo [текст]");
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

        default:
          WriteLine("Неверно набрана команда.");
          break;
      }
    }
    //ВЫБОР КОМАНДЫ

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
      WriteLine("Введите описание задачи");
      string task = ReadLine();
      tasks.Add(task);
      WriteLine("Задача добавлена!");
    }
    //ДОБАВЛЕНИЕ ЗАДАЧИ

    //ВЫВОД СПИСКА ЗАДАЧ
    private static void ShowTask()
    {
      if (tasks.Count == 0)
      {
        WriteLine("У вас нет задач");
        return;
      }

      WriteLine("Список задач:\n");

      for (int i = 0; i < tasks.Count; i++)
      {
        WriteLine($"{i + 1}. {tasks[i]}");
      }
    }
    //ВЫВОД СПИСКА ЗАДАЧ

    //УДАЛЕНИЕ ЗАДАЧИ 
    private static void RemoveTask()
    {
      if (tasks.Count == 0)
      {
        WriteLine("Список задач пуст. Удалять нечего");
        return;
      }

      ShowTask();

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
  }
}