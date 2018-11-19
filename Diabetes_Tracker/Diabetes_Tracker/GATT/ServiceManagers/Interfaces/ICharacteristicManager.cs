using System.Collections.Generic;

namespace Diabetes_Tracker.GATT.ServiceManagers.Interfaces
{
    public interface ICharacteristicManager<T> where T : CharacteristicBase
    {
        void Intake(T characterstic);
    }
}