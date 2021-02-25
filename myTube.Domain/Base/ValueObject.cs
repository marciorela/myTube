using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Domain.Base
{
    public class ValueObject
    {
        public List<string> Errors { get; set; } = new List<string>();

        protected virtual void Validate()
        {
            Errors.Clear();
        }

        public bool IsValid()
        {
            Validate();

            return Errors.Count == 0;
        }

        public bool AddErrorIfEmpty(string value, string ErrorMessage = "O campo deve ser informado.")
        {
            if (string.IsNullOrEmpty(value))
            {
                Errors.Add(ErrorMessage);
                return true;
            }
            return false;
        }
        
        public bool AddErrorIfInvalidEmail(string value, string ErrorMessage = "E-mail inválido.")
        {
            if (!string.IsNullOrEmpty(value) && !ValidaEmail(value))
            {
                Errors.Add(ErrorMessage);
                return true;
            }
            return false;
        }

        private static bool ValidaEmail(string value)
        {
            return true;
        }
    }
}
