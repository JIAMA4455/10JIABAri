using System;

namespace BonusProgram
{
    /// <summary>
    /// Базовый класс для бонусной карты
    /// Содержит общую функциональность для работы с бонусными картами
    /// </summary>
    public abstract class BonusCardBase
    {
        /// <summary>
        /// Виртуальный метод для расчета бонусов при покупке
        /// </summary>
        /// <param name="purchaseAmount">Сумма покупки</param>
        /// <returns>Количество начисляемых бонусов</returns>
        public virtual decimal CalculateBonus(decimal purchaseAmount)
        {
            return 0;
        }

        /// <summary>
        /// Виртуальный метод для проверки возможности списания бонусов
        /// </summary>
        /// <param name="bonusAmount">Количество бонусов для списания</param>
        /// <returns>true, если списание возможно</returns>
        public virtual bool CanDeductBonus(decimal bonusAmount)
        {
            return false;
        }

        /// <summary>
        /// Метод для получения информации о карте
        /// </summary>
        /// <returns>Строка с информацией о карте</returns>
        public virtual string GetCardInfo()
        {
            return "Базовая информация о карте";
        }
    }
}

