using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FunerariaWeb.Models
{
    // Tabela de junção explícita: implementa o relacionamento muitos-para-muitos
    // entre Cerimonia e Servico, guardando também a quantidade e o preço acordado
    // (o preço pode ser diferente do PrecoBase do serviço, ex. desconto).
    public class CerimoniaServico
    {
        public int CerimoniaId { get; set; }
        [ForeignKey(nameof(CerimoniaId))]
        public Cerimonia? Cerimonia { get; set; }

        public int ServicoId { get; set; }
        [ForeignKey(nameof(ServicoId))]
        public Servico? Servico { get; set; }

        [Range(1, 100)]
        public int Quantidade { get; set; } = 1;

        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Preço unitário (€)")]
        public decimal PrecoUnitario { get; set; }
    }
}
