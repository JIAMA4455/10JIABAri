using System;

namespace BonusProgram
{
    /// <summary>
    /// Partial класс бонусной карты (первая часть)
    /// Содержит защищенные поля и свойства для доступа к ним
    /// </summary>
    public partial class BonusCard : BonusCardBase
    {
        // Защищенные поля
        protected string _cardNumber = string.Empty;
        protected string _clientName = string.Empty;
        protected string _phoneNumber = string.Empty;
        protected decimal _balance;
        protected DateTime _registrationDate;
        protected bool _isActive;

        /// <summary>
        /// Номер бонусной карты
        /// </summary>
        public string CardNumber
        {
            get => _cardNumber;
            set => _cardNumber = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string ClientName
        {
            get => _clientName;
            set => _clientName = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Номер телефона клиента
        /// </summary>
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => _phoneNumber = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Баланс бонусов на карте
        /// </summary>
        public decimal Balance
        {
            get => _balance;
            internal set => _balance = value >= 0 ? value : throw new ArgumentException("Баланс не может быть отрицательным");
        }

        /// <summary>
        /// Дата регистрации карты
        /// </summary>
        public DateTime RegistrationDate
        {
            get => _registrationDate;
            internal set => _registrationDate = value;
        }

        /// <summary>
        /// Статус активности карты
        /// </summary>
        public bool IsActive
        {
            get => _isActive;
            set => _isActive = value;
        }
    }
}

