# 🚀 Консольное приложение для регистрации пользователей

Проект представляет собой консольное приложение на C#, которое:
- Регистрирует пользователей (имя и возраст)
- Проверяет валидность данных с помощью делегатов
- Определяет совершеннолетие (18+ лет)
- Генерирует событие после успешной регистрации
- Сохраняет данные совершеннолетних пользователей в файл

## 🌟 Особенности

- ✔️ Полная поддержка кириллицы  
- ✔️ Валидация ввода через делегаты  
- ✔️ Событийно-ориентированная архитектура  
- ✔️ Логирование в файл `users.txt`  
- ✔️ Гибкая система обработки ошибок  

## 📋 Требования

- [.NET 6.0](https://dotnet.microsoft.com/download/dotnet/6.0) или новее
- Консоль с поддержкой UTF-8 (для корректного отображения кириллицы)

## 🛠️ Установка и запуск

1. **Скопируйте код** из раздела [Полная реализация](#-полная-реализация) в файл `Program.cs`

2. **Создайте новый проект**:
```bash
dotnet new console -n UserRegistration
cd UserRegistration
```

3. **Замените содержимое Program.cs** на мой код

4. **Запустите приложение**:
```bash
dotnet run
```

## 🎮 Как пользоваться

После запуска программа будет запрашивать:
1. Ваше имя (только буквы, пробелы и дефисы)
2. Ваш возраст (положительное число до 120)

Пример работы:
```
Введите ваше имя:
Алексей
Введите ваш возраст:
25

--- Пользователь успешно создан ---
Приветствуем, Алексей!
Ваш возраст: 25
Статус: Совершеннолетний
Ваши данные сохранены в файл users.txt
```

## 📂 Структура проекта

```
UserRegistration/
├── Program.cs          # Основной код приложения
├── users.txt           # Автоматически создаваемый файл с данными
└── UserRegistration.csproj
```

## 📝 Полная реализация

<details>
<summary>🔍 Нажмите чтобы увидеть полный код</summary>

```csharp
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
            string name = Console.ReadLine();

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
                string input = Console.ReadLine();
                
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
            return null;
        };

        manager.CreateUser(validator);
        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}
```
</details>

## 📌 Дополнительные настройки

Для корректного отображения кириллицы:
1. **Windows**:
   - Правой кнопкой на заголовок консоли → Свойства
   - Выберите шрифт "Lucida Console"
   - Установите кодировку UTF-8

2. **Linux/macOS**:
   ```bash
   export LANG=en_US.UTF-8
   ```

## 📜 Лицензия

MIT License. См. файл [LICENSE](LICENSE).

---

<div align="center">
  <sub>Создано с ❤️ | Обновлено: 2023</sub>
</div>
```

Этот README включает:
1. Красивое форматирование с emoji 🎉
2. Пошаговые инструкции по запуску
3. Полный код с подсветкой синтаксиса
4. Примеры работы программы
5. Дополнительные настройки для разных ОС
6. Четкую структуру с разделами

Для использования просто сохраните как `README.md` в корне проекта.