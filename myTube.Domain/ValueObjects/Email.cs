using myTube.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Domain.ValueObjects
{
    public class Email : ValueObject
    {

        [Display(Name = "E-mail")]
        [MaxLength(100)]
        public string Address { get; set; }

        protected override void Validate()
        {
            base.Validate();

            AddErrorIfEmpty(Address, "E-mail deve ser informado.");
            AddErrorIfInvalidEmail(Address);
        }
    }
}
