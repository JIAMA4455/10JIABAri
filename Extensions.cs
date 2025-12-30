using System;
using System.Collections.Generic;
using System.Linq;

namespace BonusProgram
{
    /// <summary>
    /// Класс с методами расширения
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Метод расширения для строки - проверка валидности номера телефона
        /// </summary>
        public static bool IsValidPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Проверка формата: +375445123443 (плюс и 12 цифр, любые цифры)
            return System.Text.RegularExpressions.Regex.IsMatch(
                phoneNumber,
                @"^\+\d{12}$"
            );
        }

        /// <summary>
        /// Метод расширения для строки - форматирование номера телефона
        /// </summary>
        public static string FormatPhoneNumber(this string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return string.Empty;

            // Если номер уже в правильном формате (+ и 12 цифр), возвращаем как есть
            if (IsValidPhoneNumber(phoneNumber))
                return phoneNumber;

            // Удаляем все нецифровые символы кроме плюса в начале
            string cleaned = phoneNumber.Trim();
            if (cleaned.StartsWith("+"))
            {
                string digits = new string(cleaned.Substring(1).Where(char.IsDigit).ToArray());
                if (digits.Length == 12)
                {
                    return $"+{digits}";
                }
            }
            else
            {
                // Если нет плюса, добавляем его если есть 12 цифр
                string digits = new string(cleaned.Where(char.IsDigit).ToArray());
                if (digits.Length == 12)
                {
                    return $"+{digits}";
                }
            }

            return phoneNumber;
        }

        /// <summary>
        /// Метод расширения для коллекции BonusCard - получение карты по номеру
        /// </summary>
        public static BonusCard? FindByCardNumber(this IEnumerable<BonusCard> cards, string cardNumber)
        {
            return cards.FirstOrDefault(c => c.CardNumber == cardNumber && c.IsActive);
        }

        /// <summary>
        /// Метод расширения для коллекции BonusCard - получение всех активных карт
        /// </summary>
        public static IEnumerable<BonusCard> GetActiveCards(this IEnumerable<BonusCard> cards)
        {
            return cards.Where(c => c.IsActive);
        }

        /// <summary>
        /// Метод расширения для коллекции BonusCard - получение карт с балансом выше указанного
        /// </summary>
        public static IEnumerable<BonusCard> GetCardsWithBalanceAbove(this IEnumerable<BonusCard> cards, decimal minBalance)
        {
            return cards.Where(c => c.Balance >= minBalance);
        }

        /// <summary>
        /// Метод расширения для коллекции BonusCard - получение общей суммы бонусов
        /// </summary>
        public static decimal GetTotalBonuses(this IEnumerable<BonusCard> cards)
        {
            return cards.Sum(c => c.Balance);
        }

        /// <summary>
        /// Метод расширения для строки - проверка валидности имени (ФИО)
        /// </summary>
        public static bool IsValidName(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Имя должно содержать только буквы, пробелы, дефисы и апострофы
            return System.Text.RegularExpressions.Regex.IsMatch(
                name,
                @"^[А-Яа-яA-Za-z\s\-']+$"
            ) && name.Trim().Length >= 2;
        }
    }
}

