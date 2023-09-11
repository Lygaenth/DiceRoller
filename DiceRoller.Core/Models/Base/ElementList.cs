using DiceRoller.Core.Exceptions;

namespace DiceRoller.Core.Models.Base
{
    public class ElementList<T> : List<T> where T : Element
    {
        /// <summary>
        /// Guards against invalid ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UnknownIdException"></exception>
        public void GuardsAgaindInvalidID(int id)
        {
            if (!this.Any(e => e.ID == id))
                throw new UnknownIdException(GetType().ToString(), id);
        }

        /// <summary>
        /// Get first element by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetFirst(int id)
        {
            GuardsAgaindInvalidID(id);
            return this.First(e => e.ID == id);
        }
    }
}
