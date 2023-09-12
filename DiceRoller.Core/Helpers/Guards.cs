using DiceRoller.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceRoller.Core.Helpers
{
    static class Guards
    {
        /// <summary>
        /// Check parameter value
        /// </summary>
        /// <param name="valueStr"></param>
        /// <returns></returns>
        /// <exception cref="InvalidArgumentException"></exception>
        public static int AgainstNonIntParam(string valueStr)
        {
            if (valueStr != null && Int32.TryParse(valueStr, out var value))
                return value;

            throw new InvalidArgumentException(typeof(int), valueStr);
        }
    }
}
