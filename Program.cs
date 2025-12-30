using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BonusProgram
{
    /// <summary>
    /// Главный класс программы - система ведения бонусной программы магазина
    /// </summary>
    class Program
    {
        private static BonusCardRepository _repository = null!;
        private static List<BonusCard> _cards = new List<BonusCard>();
        private static OperationHistory _operationHistory = new OperationHistory();
        private const string XML_FILE_PATH = "bonus_cards.xml";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            // Подписка на события
            BonusProgramEvents.OnBonusOperation += EventHandlers.LogOperation;
            BonusProgramEvents.OnCardRegistered += EventHandlers.WelcomeNewCard;

            // Инициализация репозитория
            _repository = new BonusCardRepository(XML_FILE_PATH);
            _cards = _repository.LoadAllCards();

            Console.WriteLine("=== Система ведения бонусной программы магазина ===");
            Console.WriteLine();

            bool exit = false;
            while (!exit)
            {
                ShowMainMenu();
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RegisterNewCard();
                        break;
                    case "2":
                        ProcessPurchase();
                        break;
                    case "3":
                        DeductBonuses();
                        break;
                    case "4":
                        ViewBalance();
                        break;
                    case "5":
                        GenerateReport();
                        break;
                    case "6":
                        ViewAllCards();
                        break;
                    case "0":
                        exit = true;
                        Console.WriteLine("До свидания!");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        /// <summary>
        /// Отображение главного меню
        /// </summary>
        static void ShowMainMenu()
        {
            Console.WriteLine("Главное меню:");
            Console.WriteLine("1. Зарегистрировать новую бонусную карту");
            Console.WriteLine("2. Совершить покупку (начислить бонусы)");
            Console.WriteLine("3. Снять бонусы в счет оплаты");
            Console.WriteLine("4. Просмотреть баланс бонусов");
            Console.WriteLine("5. Сформировать отчет по карте");
            Console.WriteLine("6. Просмотреть все карты");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие: ");
        }

        /// <summary>
        /// Регистрация новой бонусной карты
        /// </summary>
        static void RegisterNewCard()
        {
            Console.WriteLine("\n=== Регистрация новой бонусной карты ===");

            string cardNumber = BonusCard.GenerateCardNumber();
            Console.WriteLine($"Сгенерированный номер карты: {cardNumber}");

            Console.Write("Введите ФИО клиента: ");
            string? clientName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(clientName) || !clientName.IsValidName())
            {
                Console.WriteLine("Ошибка: Некорректное имя.");
                return;
            }

            Console.Write("Введите номер телефона: ");
            string? phoneNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(phoneNumber) || !phoneNumber.IsValidPhoneNumber())
            {
                Console.WriteLine("Ошибка: Некорректный номер телефона.");
                return;
            }

            // Валидация через делегат
            CardValidationHandler validator = EventHandlers.ValidateCardData;
            if (!validator(cardNumber, clientName, phoneNumber))
            {
                return;
            }

            try
            {
                var newCard = new BonusCard(cardNumber, clientName, phoneNumber.FormatPhoneNumber());
                _cards.Add(newCard);
                _repository.SaveCard(newCard);

                // Вызов события через делегат
                BonusProgramEvents.RaiseCardRegistered(newCard);

                Console.WriteLine($"\nКарта успешно зарегистрирована!");
                Console.WriteLine(newCard.GetCardInfo());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при регистрации карты: {ex.Message}");
            }
        }

        /// <summary>
        /// Обработка покупки и начисление бонусов
        /// </summary>
        static void ProcessPurchase()
        {
            Console.WriteLine("\n=== Обработка покупки ===");

            Console.Write("Введите номер бонусной карты: ");
            string? cardNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(cardNumber) || !BonusCard.ValidateCardNumber(cardNumber))
            {
                Console.WriteLine("Ошибка: Некорректный номер карты.");
                return;
            }

            var card = _cards.FindByCardNumber(cardNumber);
            if (card == null)
            {
                Console.WriteLine("Карта не найдена или неактивна.");
                return;
            }

            Console.Write("Введите сумму покупки: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal purchaseAmount) || purchaseAmount <= 0)
            {
                Console.WriteLine("Ошибка: Некорректная сумма покупки.");
                return;
            }

            try
            {
                decimal bonusAmount = card.CalculateBonus(purchaseAmount);
                card.AddBonus(bonusAmount);

                var operation = new BonusOperation(
                    OperationType.Accrual,
                    card.CardNumber,
                    bonusAmount,
                    $"Начисление бонусов за покупку на сумму {purchaseAmount:F2}",
                    card.Balance
                );

                _operationHistory.AddOperation(operation);
                _repository.SaveCard(card);

                // Вызов события через делегат
                BonusProgramEvents.RaiseBonusOperation(card.CardNumber, bonusAmount, OperationType.Accrual);

                Console.WriteLine($"\nБонусы успешно начислены!");
                Console.WriteLine($"Начислено бонусов: {bonusAmount:F2}");
                Console.WriteLine($"Текущий баланс: {card.Balance:F2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при начислении бонусов: {ex.Message}");
            }
        }

        /// <summary>
        /// Снятие бонусов в счет оплаты
        /// </summary>
        static void DeductBonuses()
        {
            Console.WriteLine("\n=== Снятие бонусов в счет оплаты ===");

            Console.Write("Введите номер бонусной карты: ");
            string? cardNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(cardNumber) || !BonusCard.ValidateCardNumber(cardNumber))
            {
                Console.WriteLine("Ошибка: Некорректный номер карты.");
                return;
            }

            var card = _cards.FindByCardNumber(cardNumber);
            if (card == null)
            {
                Console.WriteLine("Карта не найдена или неактивна.");
                return;
            }

            Console.WriteLine($"Текущий баланс: {card.Balance:F2} бонусов");
            Console.Write("Введите количество бонусов для списания: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal bonusAmount) || bonusAmount <= 0)
            {
                Console.WriteLine("Ошибка: Некорректная сумма для списания.");
                return;
            }

            if (!card.CanDeductBonus(bonusAmount))
            {
                Console.WriteLine("Ошибка: Недостаточно бонусов для списания или баланс меньше минимального.");
                return;
            }

            try
            {
                if (card.DeductBonus(bonusAmount))
                {
                    var operation = new BonusOperation(
                        OperationType.Deduction,
                        card.CardNumber,
                        bonusAmount,
                        $"Списание бонусов в счет оплаты",
                        card.Balance
                    );

                    _operationHistory.AddOperation(operation);
                    _repository.SaveCard(card);

                    // Вызов события через делегат
                    BonusProgramEvents.RaiseBonusOperation(card.CardNumber, bonusAmount, OperationType.Deduction);

                    Console.WriteLine($"\nБонусы успешно списаны!");
                    Console.WriteLine($"Списано бонусов: {bonusAmount:F2}");
                    Console.WriteLine($"Текущий баланс: {card.Balance:F2}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при списании бонусов: {ex.Message}");
            }
        }

        /// <summary>
        /// Просмотр баланса бонусов
        /// </summary>
        static void ViewBalance()
        {
            Console.WriteLine("\n=== Просмотр баланса бонусов ===");

            Console.Write("Введите номер бонусной карты: ");
            string? cardNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(cardNumber) || !BonusCard.ValidateCardNumber(cardNumber))
            {
                Console.WriteLine("Ошибка: Некорректный номер карты.");
                return;
            }

            var card = _cards.FindByCardNumber(cardNumber);
            if (card == null)
            {
                Console.WriteLine("Карта не найдена или неактивна.");
                return;
            }

            Console.WriteLine("\n" + card.GetCardInfo());
        }

        /// <summary>
        /// Формирование отчета по карте
        /// </summary>
        static void GenerateReport()
        {
            Console.WriteLine("\n=== Формирование отчета по карте ===");

            Console.Write("Введите номер бонусной карты: ");
            string? cardNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(cardNumber) || !BonusCard.ValidateCardNumber(cardNumber))
            {
                Console.WriteLine("Ошибка: Некорректный номер карты.");
                return;
            }

            var card = _cards.FindByCardNumber(cardNumber);
            if (card == null)
            {
                Console.WriteLine("Карта не найдена или неактивна.");
                return;
            }

            Console.Write("Введите начальную дату (дд.мм.гггг): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
            {
                Console.WriteLine("Ошибка: Некорректная дата.");
                return;
            }

            Console.Write("Введите конечную дату (дд.мм.гггг): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
            {
                Console.WriteLine("Ошибка: Некорректная дата.");
                return;
            }

            // Использование LINQ to Objects для получения операций
            var operations = _operationHistory.GetOperationsByCardAndPeriod(cardNumber, startDate, endDate).ToList();

            if (!operations.Any())
            {
                Console.WriteLine("За указанный период операций не найдено.");
                return;
            }

            Console.Write("Введите имя файла для сохранения отчета: ");
            string? fileName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = $"report_{cardNumber}_{DateTime.Now:yyyyMMdd}.txt";
            }

            if (!fileName.EndsWith(".txt"))
            {
                fileName += ".txt";
            }

            try
            {
                // Использование LINQ to Objects для форматирования данных
                var reportLines = new List<string>
                {
                    "=== ОТЧЕТ ПО БОНУСНОЙ КАРТЕ ===",
                    "",
                    card.GetCardInfo(),
                    "",
                    $"Период: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}",
                    "",
                    "=== ИСТОРИЯ ОПЕРАЦИЙ ===",
                    ""
                };

                // Добавление операций используя LINQ
                reportLines.AddRange(operations.Select(op => op.ToString()));

                // Подсчет статистики используя LINQ
                var totalAccrual = operations.Where(op => op.Type == OperationType.Accrual).Sum(op => op.Amount);
                var totalDeduction = operations.Where(op => op.Type == OperationType.Deduction).Sum(op => op.Amount);

                reportLines.Add("");
                reportLines.Add("=== СТАТИСТИКА ===");
                reportLines.Add($"Всего операций: {operations.Count}");
                reportLines.Add($"Всего начислено: {totalAccrual:F2} бонусов");
                reportLines.Add($"Всего списано: {totalDeduction:F2} бонусов");
                reportLines.Add($"Текущий баланс: {card.Balance:F2} бонусов");

                File.WriteAllLines(fileName, reportLines);

                Console.WriteLine($"\nОтчет успешно сохранен в файл: {fileName}");
                Console.WriteLine($"Всего операций: {operations.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении отчета: {ex.Message}");
            }
        }

        /// <summary>
        /// Просмотр всех карт
        /// </summary>
        static void ViewAllCards()
        {
            Console.WriteLine("\n=== Список всех бонусных карт ===");

            // Использование LINQ to Objects для фильтрации активных карт
            var activeCards = _cards.GetActiveCards().ToList();

            if (!activeCards.Any())
            {
                Console.WriteLine("Активных карт не найдено.");
                return;
            }

            Console.WriteLine($"Всего активных карт: {activeCards.Count}");
            Console.WriteLine($"Общая сумма бонусов: {activeCards.GetTotalBonuses():F2}");
            Console.WriteLine();

            foreach (var card in activeCards)
            {
                Console.WriteLine(card.GetCardInfo());
                Console.WriteLine("---");
            }
        }
    }
}

