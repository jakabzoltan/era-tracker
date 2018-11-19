using System;
using Java.Util;
using nexus.protocols.ble;

namespace Diabetes_Tracker.GATT.Exceptions
{
    public class GattValidationException : Exception
    {
        public GattValidationException(string paramName) : base($"Validation exception with {paramName}.")
        {
            
        }
    }

    public class GattCharactersticMismatch : Exception
    {
        public GattCharactersticMismatch(CharacteristicBase concreteCharacteristicBase, UUID intendedCharacterstic) : base($"\"{concreteCharacteristicBase.GetType().Name} : {concreteCharacteristicBase.Uuid}\" is not a viable schema for the charactersitic with the UUID of \"{intendedCharacterstic}\"")
        {
            
        }
    }
}