using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiceRoller.Core.Models.Base
{
    public abstract class Element
    {
        public int ID { get; set; }

        public Element(int id)
        {
            ID = id;
        }
    }
}
