using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BonusProgram
{
    /// <summary>
    /// Класс для работы с XML файлом бонусных карт используя LINQ to XML
    /// </summary>
    public class BonusCardRepository
    {
        private readonly string _xmlFilePath;
        private XDocument _xDocument = null!;

        /// <summary>
        /// Конструктор
        /// </summary>
        public BonusCardRepository(string xmlFilePath)
        {
            _xmlFilePath = xmlFilePath ?? throw new ArgumentNullException(nameof(xmlFilePath));
            LoadXml();
        }

        /// <summary>
        /// Загрузка XML файла
        /// </summary>
        private void LoadXml()
        {
            if (File.Exists(_xmlFilePath))
            {
                _xDocument = XDocument.Load(_xmlFilePath);
            }
            else
            {
                _xDocument = new XDocument(
                    new XElement("BonusCards")
                );
            }
        }

        /// <summary>
        /// Сохранение XML файла
        /// </summary>
        private void SaveXml()
        {
            _xDocument.Save(_xmlFilePath);
        }

        /// <summary>
        /// Загрузка всех бонусных карт из XML
        /// </summary>
        public List<BonusCard> LoadAllCards()
        {
            var cards = new List<BonusCard>();

            if (_xDocument.Root == null)
                return cards;

            // Использование LINQ to XML для чтения данных
            var cardElements = from card in _xDocument.Root.Elements("BonusCard")
                               select card;

            foreach (var cardElement in cardElements)
            {
                try
                {
                    var card = new BonusCard(
                        cardElement.Element("CardNumber")?.Value ?? "",
                        cardElement.Element("ClientName")?.Value ?? "",
                        cardElement.Element("PhoneNumber")?.Value ?? ""
                    );

                    if (decimal.TryParse(cardElement.Element("Balance")?.Value, out decimal balance))
                        card.AddBonus(balance);

                    if (DateTime.TryParse(cardElement.Element("RegistrationDate")?.Value, out DateTime regDate))
                        card = new BonusCard(card.CardNumber, card.ClientName, card.PhoneNumber)
                        {
                            Balance = balance,
                            RegistrationDate = regDate
                        };

                    if (bool.TryParse(cardElement.Element("IsActive")?.Value, out bool isActive))
                        card.IsActive = isActive;

                    cards.Add(card);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при загрузке карты: {ex.Message}");
                }
            }

            return cards;
        }

        /// <summary>
        /// Сохранение бонусной карты в XML
        /// </summary>
        public void SaveCard(BonusCard card)
        {
            if (_xDocument.Root == null)
            {
                _xDocument = new XDocument(new XElement("BonusCards"));
            }

            // Проверка существования карты
            var existingCard = (from c in _xDocument.Root!.Elements("BonusCard")
                               where c.Element("CardNumber")?.Value == card.CardNumber
                               select c).FirstOrDefault();

            if (existingCard != null)
            {
                // Обновление существующей карты
                existingCard.Element("ClientName")!.Value = card.ClientName;
                existingCard.Element("PhoneNumber")!.Value = card.PhoneNumber;
                existingCard.Element("Balance")!.Value = card.Balance.ToString("F2");
                existingCard.Element("RegistrationDate")!.Value = card.RegistrationDate.ToString("yyyy-MM-dd");
                existingCard.Element("IsActive")!.Value = card.IsActive.ToString();
            }
            else
            {
                // Добавление новой карты
                var cardElement = new XElement("BonusCard",
                    new XElement("CardNumber", card.CardNumber),
                    new XElement("ClientName", card.ClientName),
                    new XElement("PhoneNumber", card.PhoneNumber),
                    new XElement("Balance", card.Balance.ToString("F2")),
                    new XElement("RegistrationDate", card.RegistrationDate.ToString("yyyy-MM-dd")),
                    new XElement("IsActive", card.IsActive.ToString())
                );

                _xDocument.Root!.Add(cardElement);
            }

            SaveXml();
        }

        /// <summary>
        /// Сохранение коллекции карт в XML
        /// </summary>
        public void SaveAllCards(IEnumerable<BonusCard> cards)
        {
            if (_xDocument.Root == null)
            {
                _xDocument = new XDocument(new XElement("BonusCards"));
            }

            // Очистка существующих карт
            _xDocument.Root!.Elements("BonusCard").Remove();

            // Использование LINQ to Objects для создания элементов
            var cardElements = cards.Select(card => new XElement("BonusCard",
                new XElement("CardNumber", card.CardNumber),
                new XElement("ClientName", card.ClientName),
                new XElement("PhoneNumber", card.PhoneNumber),
                new XElement("Balance", card.Balance.ToString("F2")),
                new XElement("RegistrationDate", card.RegistrationDate.ToString("yyyy-MM-dd")),
                new XElement("IsActive", card.IsActive.ToString())
            ));

            _xDocument.Root.Add(cardElements);
            SaveXml();
        }

        /// <summary>
        /// Поиск карты по номеру используя LINQ to XML
        /// </summary>
        public BonusCard? FindCardByNumber(string cardNumber)
        {
            if (_xDocument.Root == null)
                return null;

            var cardElement = (from card in _xDocument.Root.Elements("BonusCard")
                              where card.Element("CardNumber")?.Value == cardNumber
                              select card).FirstOrDefault();

            if (cardElement == null)
                return null;

            try
            {
                var card = new BonusCard(
                    cardElement.Element("CardNumber")?.Value ?? "",
                    cardElement.Element("ClientName")?.Value ?? "",
                    cardElement.Element("PhoneNumber")?.Value ?? ""
                );

                if (decimal.TryParse(cardElement.Element("Balance")?.Value, out decimal balance))
                    card.AddBonus(balance);

                if (DateTime.TryParse(cardElement.Element("RegistrationDate")?.Value, out DateTime regDate))
                {
                    card = new BonusCard(card.CardNumber, card.ClientName, card.PhoneNumber)
                    {
                        Balance = balance,
                        RegistrationDate = regDate
                    };
                }

                if (bool.TryParse(cardElement.Element("IsActive")?.Value, out bool isActive))
                    card.IsActive = isActive;

                return card;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Получение всех активных карт используя LINQ to XML
        /// </summary>
        public IEnumerable<XElement> GetActiveCardsXml()
        {
            if (_xDocument.Root == null)
                return Enumerable.Empty<XElement>();

            return from card in _xDocument.Root.Elements("BonusCard")
                   where card.Element("IsActive")?.Value == "True"
                   select card;
        }
    }
}

