using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Domain
{
    public class MensagemDto
    {
        public string Mensagem { get; set; } = "Hello Word";
        public Guid Processo { get; set; }
        public Guid Identificador { get; set; } = Guid.NewGuid();

        public MensagemDto(Guid processo)
        {
            Processo = processo;
        }

        public MensagemDto()
        {

        }

    }
}

