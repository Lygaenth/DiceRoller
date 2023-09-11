namespace DiceRoller.Core.Exceptions
{
    public class InvalidArgumentException : Exception
    {
        private readonly Type _expectedType;
        private readonly string? _wrongValue;

        public InvalidArgumentException(Type expectedType, string? wrongValue)
        {
            _expectedType = expectedType;
            _wrongValue = wrongValue;
        }

        public override string ToString()
        {
            if (_wrongValue == null)
                return "Null value not expected";
            return "Failed to convert " + _wrongValue + "into " + _expectedType.ToString();
        }

        public override string Message => ToString();
    }
}
