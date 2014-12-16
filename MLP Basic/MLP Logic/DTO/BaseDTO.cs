using System.Collections.Generic;

namespace MLP_Logic.DTO
{
    public abstract class BaseDTO
    {
        public bool IsValid { get { return !(Exceptions.Count > 0); } }
        public List<string> Exceptions { get { return exceptions; } }

        protected List<string> exceptions;

        public BaseDTO()
        {
            exceptions = new List<string>();
        }
    }
}
