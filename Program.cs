using System;
using System.IO;
using System.Text;

class Program
{
    delegate string NameValidator(string name);

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

    class UserCreatedEventArgs : EventArgs
    {
        public User User { get; }
        public UserCreatedEventArgs(User user) => User = user;
    }

    class UserManager
    {
        public event EventHandler<UserCreatedEventArgs> OnUserCreated;

        public void CreateUser(NameValidator validator)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.WriteLine("Введите ваше имя:");
            string name = Console.ReadLine()!;

            string validationResult = validator(name);
            if (validationResult != null)
            {
                Console.WriteLine(validationResult);
                return;
            }

            int age = GetValidAge();
            var user = new User(name, age);
            OnUserCreated?.Invoke(this, new UserCreatedEventArgs(user));
        }

        private int GetValidAge()
        {
            int age;
            while (true)
            {
                Console.WriteLine("Введите ваш возраст:");
                string input = Console.ReadLine()!;
                
                if (!int.TryParse(input, out age))
                    Console.WriteLine("Ошибка: Возраст должен быть числом!");
                else if (age <= 0)
                    Console.WriteLine("Ошибка: Возраст должен быть положительным числом!");
                else if (age > 120)
                    Console.WriteLine("Ошибка: Введите реалистичный возраст!");
                else
                    break;
            }
            return age;
        }
    }

    static void Main()
    {
        var manager = new UserManager();
        manager.OnUserCreated += (sender, e) =>
        {
            Console.WriteLine("\n--- Пользователь успешно создан ---");
            Console.WriteLine($"Приветствуем, {e.User.Name}!");
            Console.WriteLine($"Ваш возраст: {e.User.Age}");
            Console.WriteLine($"Статус: {(e.User.IsAdult ? "Совершеннолетний" : "Несовершеннолетний")}");

            if (e.User.IsAdult)
            {
                try
                {
                    File.AppendAllText("users.txt", $"{DateTime.Now}: {e.User.Name}, {e.User.Age} лет\n");
                    Console.WriteLine("Ваши данные сохранены в файл users.txt");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
                }
            }
        };

        NameValidator validator = name =>
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Ошибка: Имя не может быть пустым!";
            if (name.Length < 2)
                return "Ошибка: Имя слишком короткое!";
            foreach (char c in name)
                if (!char.IsLetter(c) && c != ' ' && c != '-')
                    return "Ошибка: Имя должно содержать только буквы, пробелы или дефисы!";
            return null!;
        };

        manager.CreateUser(validator);
        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}