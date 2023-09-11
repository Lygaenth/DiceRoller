namespace DiceRoller.Core.Exceptions
{
    public class UnknownIdException : Exception
    {
        private readonly int _id;
        private readonly string _objectType;

        public UnknownIdException(string objectType, int objectId)
        {
            _objectType = objectType;
            _id = objectId;
        }

        public override string ToString()
        {
            return "Unable to find object " + _objectType + " with id " + _id;
        }

        public override string Message => this.ToString();
    }
}
