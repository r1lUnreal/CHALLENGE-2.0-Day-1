using System;
using System.IO;
using System.Text;

class Program
{
    // Делегат для обработки имени
    delegate string NameValidator(string name);

    // Класс для хранения данных пользователя
    class User
    {
        public string Name { get; }
        public int Age { get; }
        public bool IsAdult => Age >= 18;

        public User(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }

    // Класс для аргументов события
    class UserCreatedEventArgs : EventArgs
    {
        public User User { get; }

        public UserCreatedEventArgs(User user)
        {
            User = user;
        }
    }

    // Класс для управления пользователями
    class UserManager
    {
        // Событие создания пользователя
        public event EventHandler<UserCreatedEventArgs> OnUserCreated = null!;

        // Метод для создания пользователя
        public void CreateUser(NameValidator validator)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            PrintHeader("РЕГИСТРАЦИЯ ПОЛЬЗОВАТЕЛЯ", ConsoleColor.DarkCyan);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Введите ваше имя:");
            Console.ResetColor();
            string name = Console.ReadLine()!;

            // Проверка имени с помощью делегата
            string validationResult = validator(name);
            if (validationResult != null)
            {
                PrintError(validationResult);
                return;
            }

            int age = GetValidAge();

            var user = new User(name, age);

            // Вызываем событие
            OnUserCreated?.Invoke(this, new UserCreatedEventArgs(user));
        }

        private int GetValidAge()
        {
            int age;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Введите ваш возраст:");
                Console.ResetColor();
                string input = Console.ReadLine()!;

                if (!int.TryParse(input, out age))
                {
                    PrintError("Ошибка: Возраст должен быть числом!");
                    continue;
                }

                if (age <= 0)
                {
                    PrintError("Ошибка: Возраст должен быть положительным числом!");
                    continue;
                }

                if (age > 120)
                {
                    PrintError("Ошибка: Введите реалистичный возраст!");
                    continue;
                }

                break;
            }
            return age;
        }

        private void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private void PrintHeader(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine("====================================");
            Console.WriteLine($" {message}");
            Console.WriteLine("====================================");
            Console.ResetColor();
        }
    }

    static void Main()
    {
        var manager = new UserManager();

        // Подписываемся на событие
        manager.OnUserCreated += HandleUserCreated!;

        // Создаем делегат для валидации имени
        NameValidator validator = name =>
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Ошибка: Имя не может быть пустым!";
            }

            if (name.Length < 2)
            {
                return "Ошибка: Имя слишком короткое!";
            }

            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != ' ' && c != '-')
                {
                    return "Ошибка: Имя должно содержать только английскую раскладку!";
                }
            }

            return null!;
        };

        // Создаем пользователя
        manager.CreateUser(validator);

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ResetColor();
        Console.ReadKey();
    }

    // Обработчик события создания пользователя
    private static void HandleUserCreated(object sender, UserCreatedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n════════════════════════════════════");
        Console.WriteLine("   ПОЛЬЗОВАТЕЛЬ УСПЕШНО СОЗДАН");
        Console.WriteLine("════════════════════════════════════");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"👋 Приветствуем, {e.User.Name}!");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"🔞 Возраст: {e.User.Age} лет");
        Console.WriteLine($"📌 Статус: {(e.User.IsAdult ? "Совершеннолетний" : "Несовершеннолетний")}");
        Console.ResetColor();

        if (e.User.IsAdult)
        {
            try
            {
                File.AppendAllText("users.txt", $"{DateTime.Now}: {e.User.Name}, {e.User.Age} лет\n");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("💾 Ваши данные сохранены в файл users.txt");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Ошибка при сохранении в файл: {ex.Message}");
                Console.ResetColor();
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("════════════════════════════════════\n");
        Console.ResetColor();
    }
}