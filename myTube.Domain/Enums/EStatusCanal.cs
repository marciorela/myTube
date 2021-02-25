using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Domain.Enums
{
    public enum EStatusCanal
    {
        Ativo = 0,
        Inativo = 1,

        [Description("Validando")]
        Validando = 2
    }
}
