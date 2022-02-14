using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Domain.Enums
{
    public enum EStatusVideo
    {
        [Description("Não Assistido")]
        NaoAssistido = 0,

        Assistido = 1,

        Ignorado = 2,

        Favorito = 3,

        [Description("Assistir depois")]
        AssistirDepois = 4,

        Cancelado = 5
    }
}
