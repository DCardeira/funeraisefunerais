using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FunerariaWeb.Models
{
    // O "processo" de acompanhamento de um funeral. É o centro do modelo:
    // liga-se a UM Cliente (muitos-para-um) e a VÁRIOS Serviços (muitos-para-muitos).
    public class Cerimonia
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Selecione o cliente.")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }

        [Required(ErrorMessage = "A data da cerimónia é obrigatória.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Data e hora")]
        public DateTime DataCerimonia { get; set; } = DateTime.Now.AddDays(1);

        [Required(ErrorMessage = "O local é obrigatório.")]
        [StringLength(150)]
        public string Local { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Estado")]
        public EstadoCerimonia Estado { get; set; } = EstadoCerimonia.Pendente;

        [StringLength(1000)]
        public string? Observacoes { get; set; }

        // Relação muitos-para-muitos com Servico, através desta tabela de junção.
        public ICollection<CerimoniaServico> CerimoniaServicos { get; set; } = new List<CerimoniaServico>();

        [NotMapped]
        public decimal Total => CerimoniaServicos?.Sum(cs => cs.PrecoUnitario * cs.Quantidade) ?? 0;
    }

    public enum EstadoCerimonia
    {
        [Display(Name = "Pendente")]
        Pendente,
        [Display(Name = "Confirmada")]
        Confirmada,
        [Display(Name = "Concluída")]
        Concluida,
        [Display(Name = "Cancelada")]
        Cancelada
    }
}
