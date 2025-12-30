using System;

namespace BonusProgram
{
    /// <summary>
    /// Partial класс бонусной карты (вторая часть)
    /// Содержит методы, константы, read-only поля, статические поля и методы
    /// </summary>
    public partial class BonusCard
    {
        // Константы
        public const decimal DEFAULT_BONUS_PERCENT = 5.0m; // 5% от суммы покупки
        public const decimal MIN_BALANCE_FOR_DEDUCTION = 100.0m; // Минимальный баланс для списания
        public const int CARD_NUMBER_LENGTH = 16;

        // Read-only поля
        public readonly DateTime CreatedAt;
        public readonly string CardType = "Standard";

        // Статические поля
        private static int _totalCardsCreated = 0;
        private static decimal _totalBonusesIssued = 0;

        /// <summary>
        /// Статическое свойство для получения общего количества созданных карт
        /// </summary>
        public static int TotalCardsCreated => _totalCardsCreated;

        /// <summary>
        /// Статическое свойство для получения общего количества выданных бонусов
        /// </summary>
        public static decimal TotalBonusesIssued => _totalBonusesIssued;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public BonusCard(string cardNumber, string clientName, string phoneNumber)
        {
            CardNumber = cardNumber;
            ClientName = clientName;
            PhoneNumber = phoneNumber;
            Balance = 0;
            RegistrationDate = DateTime.Now;
            CreatedAt = DateTime.Now;
            IsActive = true;
            _totalCardsCreated++;
        }

        /// <summary>
        /// Переопределение виртуального метода для расчета бонусов
        /// </summary>
        public override decimal CalculateBonus(decimal purchaseAmount)
        {
            if (purchaseAmount <= 0)
                throw new ArgumentException("Сумма покупки должна быть положительной");

            if (!IsActive)
                return 0;

            decimal bonus = purchaseAmount * (DEFAULT_BONUS_PERCENT / 100);
            _totalBonusesIssued += bonus;
            return bonus;
        }

        /// <summary>
        /// Переопределение виртуального метода для проверки возможности списания
        /// </summary>
        public override bool CanDeductBonus(decimal bonusAmount)
        {
            return IsActive && Balance >= bonusAmount && Balance >= MIN_BALANCE_FOR_DEDUCTION;
        }

        /// <summary>
        /// Переопределение метода получения информации о карте
        /// </summary>
        public override string GetCardInfo()
        {
            return $"Карта №{CardNumber}\n" +
                   $"Владелец: {ClientName}\n" +
                   $"Телефон: {PhoneNumber}\n" +
                   $"Баланс: {Balance:F2} бонусов\n" +
                   $"Дата регистрации: {RegistrationDate:dd.MM.yyyy}\n" +
                   $"Статус: {(IsActive ? "Активна" : "Неактивна")}";
        }

        /// <summary>
        /// Метод для начисления бонусов
        /// </summary>
        public void AddBonus(decimal bonusAmount)
        {
            if (bonusAmount <= 0)
                throw new ArgumentException("Сумма бонусов должна быть положительной");

            Balance += bonusAmount;
        }

        /// <summary>
        /// Метод для списания бонусов
        /// </summary>
        public bool DeductBonus(decimal bonusAmount)
        {
            if (!CanDeductBonus(bonusAmount))
                return false;

            Balance -= bonusAmount;
            return true;
        }

        /// <summary>
        /// Статический метод для валидации номера карты
        /// </summary>
        public static bool ValidateCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return false;

            if (cardNumber.Length != CARD_NUMBER_LENGTH)
                return false;

            return System.Text.RegularExpressions.Regex.IsMatch(cardNumber, @"^\d+$");
        }

        /// <summary>
        /// Статический метод для генерации номера карты
        /// </summary>
        public static string GenerateCardNumber()
        {
            Random random = new Random();
            string number = "";
            for (int i = 0; i < CARD_NUMBER_LENGTH; i++)
            {
                number += random.Next(0, 10).ToString();
            }
            return number;
        }

        /// <summary>
        /// Статический метод для сброса статистики (для тестирования)
        /// </summary>
        public static void ResetStatistics()
        {
            _totalCardsCreated = 0;
            _totalBonusesIssued = 0;
        }
    }
}

