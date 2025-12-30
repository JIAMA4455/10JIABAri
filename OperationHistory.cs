using System;
using System.Collections.Generic;
using System.Linq;

namespace BonusProgram
{
    /// <summary>
    /// Класс для управления историей операций с бонусными картами
    /// </summary>
    public class OperationHistory
    {
        private readonly List<BonusOperation> _operations;

        /// <summary>
        /// Конструктор
        /// </summary>
        public OperationHistory()
        {
            _operations = new List<BonusOperation>();
        }

        /// <summary>
        /// Добавление операции в историю
        /// </summary>
        public void AddOperation(BonusOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            _operations.Add(operation);
        }

        /// <summary>
        /// Получение всех операций
        /// </summary>
        public IEnumerable<BonusOperation> GetAllOperations()
        {
            return _operations.AsEnumerable();
        }

        /// <summary>
        /// Получение операций по номеру карты
        /// </summary>
        public IEnumerable<BonusOperation> GetOperationsByCard(string cardNumber)
        {
            return _operations.Where(op => op.CardNumber == cardNumber);
        }

        /// <summary>
        /// Получение операций за период
        /// </summary>
        public IEnumerable<BonusOperation> GetOperationsByPeriod(DateTime startDate, DateTime endDate)
        {
            return _operations.Where(op => op.OperationDate >= startDate && op.OperationDate <= endDate);
        }

        /// <summary>
        /// Получение операций по карте за период
        /// </summary>
        public IEnumerable<BonusOperation> GetOperationsByCardAndPeriod(string cardNumber, DateTime startDate, DateTime endDate)
        {
            return _operations
                .Where(op => op.CardNumber == cardNumber 
                          && op.OperationDate >= startDate 
                          && op.OperationDate <= endDate)
                .OrderBy(op => op.OperationDate);
        }

        /// <summary>
        /// Получение операций определенного типа
        /// </summary>
        public IEnumerable<BonusOperation> GetOperationsByType(OperationType type)
        {
            return _operations.Where(op => op.Type == type);
        }

        /// <summary>
        /// Получение количества операций
        /// </summary>
        public int GetOperationsCount()
        {
            return _operations.Count;
        }

        /// <summary>
        /// Очистка истории
        /// </summary>
        public void Clear()
        {
            _operations.Clear();
        }
    }
}

