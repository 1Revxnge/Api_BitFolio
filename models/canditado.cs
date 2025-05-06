using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiJobfy.models
{
    public class Candidato : Usuario
    {
        [Required]
        public byte[] CurriculoCriptografado { get; set; } = Array.Empty<byte>();

        // Anonimizar currículo (limpar dados sensíveis eventualmente)
        public override void Anonimizar()
        {
            base.Anonimizar();
            CurriculoCriptografado = Array.Empty<byte>(); // opção de limpar currículo no delete
        }
    }
}
