using System;

namespace BonusProgram
{
    /// <summary>
    /// Класс для представления операции с бонусной картой
    /// </summary>
    public class BonusOperation
    {
        /// <summary>
        /// Тип операции
        /// </summary>
        public OperationType Type { get; set; }

        /// <summary>
        /// Номер карты
        /// </summary>
        public string CardNumber { get; set; } = string.Empty;

        /// <summary>
        /// Сумма операции
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Дата и время операции
        /// </summary>
        public DateTime OperationDate { get; set; }

        /// <summary>
        /// Описание операции
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Баланс после операции
        /// </summary>
        public decimal BalanceAfter { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public BonusOperation(OperationType type, string cardNumber, decimal amount, string description, decimal balanceAfter)
        {
            Type = type;
            CardNumber = cardNumber;
            Amount = amount;
            Description = description;
            BalanceAfter = balanceAfter;
            OperationDate = DateTime.Now;
        }

        /// <summary>
        /// Конструктор по умолчанию для десериализации
        /// </summary>
        public BonusOperation()
        {
            OperationDate = DateTime.Now;
        }

        /// <summary>
        /// Переопределение ToString для удобного вывода
        /// </summary>
        public override string ToString()
        {
            string typeStr = Type == OperationType.Accrual ? "Начисление" : "Списание";
            return $"{OperationDate:dd.MM.yyyy HH:mm:ss} | {typeStr} | Карта: {CardNumber} | Сумма: {Amount:F2} | Баланс: {BalanceAfter:F2} | {Description}";
        }
    }

    /// <summary>
    /// Тип операции с бонусами
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// Начисление бонусов
        /// </summary>
        Accrual,

        /// <summary>
        /// Списание бонусов
        /// </summary>
        Deduction
    }
}

