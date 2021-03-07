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
        NaoAssistido,

        Assistido,

        Ignorado,

        Favorito,

        [Description("Assistir depois")]
        AssistirDepois
    }
}
