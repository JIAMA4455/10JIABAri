using System;

namespace BonusProgram
{
    /// <summary>
    /// Делегат для обработки событий операций с бонусной картой
    /// </summary>
    /// <param name="cardNumber">Номер карты</param>
    /// <param name="amount">Сумма операции</param>
    /// <param name="operationType">Тип операции</param>
    public delegate void BonusOperationHandler(string cardNumber, decimal amount, OperationType operationType);

    /// <summary>
    /// Делегат для обработки событий регистрации новой карты
    /// </summary>
    /// <param name="card">Зарегистрированная карта</param>
    public delegate void CardRegisteredHandler(BonusCard card);

    /// <summary>
    /// Делегат для валидации данных карты
    /// </summary>
    /// <param name="cardNumber">Номер карты</param>
    /// <param name="clientName">Имя клиента</param>
    /// <param name="phoneNumber">Номер телефона</param>
    /// <returns>true, если данные валидны</returns>
    public delegate bool CardValidationHandler(string cardNumber, string clientName, string phoneNumber);

    /// <summary>
    /// Класс для управления событиями бонусной программы
    /// </summary>
    public class BonusProgramEvents
    {
        /// <summary>
        /// Событие операции с бонусами
        /// </summary>
        public static event BonusOperationHandler? OnBonusOperation;

        /// <summary>
        /// Событие регистрации новой карты
        /// </summary>
        public static event CardRegisteredHandler? OnCardRegistered;

        /// <summary>
        /// Вызов события операции с бонусами
        /// </summary>
        public static void RaiseBonusOperation(string cardNumber, decimal amount, OperationType operationType)
        {
            OnBonusOperation?.Invoke(cardNumber, amount, operationType);
        }

        /// <summary>
        /// Вызов события регистрации карты
        /// </summary>
        public static void RaiseCardRegistered(BonusCard card)
        {
            OnCardRegistered?.Invoke(card);
        }
    }

    /// <summary>
    /// Класс с обработчиками событий
    /// </summary>
    public static class EventHandlers
    {
        /// <summary>
        /// Обработчик операции с бонусами - логирование
        /// </summary>
        public static void LogOperation(string cardNumber, decimal amount, OperationType operationType)
        {
            string operationTypeStr = operationType == OperationType.Accrual ? "начисление" : "списание";
            Console.WriteLine($"[ЛОГ] Операция: {operationTypeStr} {amount:F2} бонусов на карту {cardNumber}");
        }

        /// <summary>
        /// Обработчик регистрации карты - приветствие
        /// </summary>
        public static void WelcomeNewCard(BonusCard card)
        {
            Console.WriteLine($"[ПРИВЕТСТВИЕ] Добро пожаловать в бонусную программу, {card.ClientName}!");
            Console.WriteLine($"Ваша карта №{card.CardNumber} успешно зарегистрирована.");
        }

        /// <summary>
        /// Обработчик валидации карты
        /// </summary>
        public static bool ValidateCardData(string cardNumber, string clientName, string phoneNumber)
        {
            if (!BonusCard.ValidateCardNumber(cardNumber))
            {
                Console.WriteLine("Ошибка: Некорректный номер карты. Номер должен содержать 16 цифр.");
                return false;
            }

            if (!clientName.IsValidName())
            {
                Console.WriteLine("Ошибка: Некорректное имя. Имя должно содержать только буквы.");
                return false;
            }

            if (!phoneNumber.IsValidPhoneNumber())
            {
                Console.WriteLine("Ошибка: Некорректный номер телефона.");
                return false;
            }

            return true;
        }
    }
}

