using System.ComponentModel.DataAnnotations;
using FunerariaWeb.Models;

namespace FunerariaWeb.Pages.Cerimonias
{
    // Agrupa os campos da Cerimonia com a lista de Serviços disponíveis,
    // para que a página de Criar/Editar consiga apresentar checkboxes
    // e gerir a tabela de junção CerimoniaServico num único formulário.
    public class CerimoniaFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Selecione o cliente.")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "A data da cerimónia é obrigatória.")]
        [Display(Name = "Data e hora")]
        public DateTime DataCerimonia { get; set; } = DateTime.Now.AddDays(1);

        [Required(ErrorMessage = "O local é obrigatório.")]
        [StringLength(150)]
        public string Local { get; set; } = string.Empty;

        [Required]
        public EstadoCerimonia Estado { get; set; } = EstadoCerimonia.Pendente;

        [StringLength(1000)]
        public string? Observacoes { get; set; }

        public List<ServicoSelecionavel> ServicosDisponiveis { get; set; } = new();
    }

    public class ServicoSelecionavel
    {
        public int ServicoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal PrecoBase { get; set; }
        public bool Selecionado { get; set; }
        [Range(1, 100)]
        public int Quantidade { get; set; } = 1;
    }
}
