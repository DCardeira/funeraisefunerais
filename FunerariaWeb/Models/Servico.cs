using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FunerariaWeb.Models
{
    // Um serviço/produto que a funerária disponibiliza (velório, cremação, transporte, flores, ...).
    public class Servico
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do serviço é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descricao { get; set; }

        [Required]
        [Range(0, 100000, ErrorMessage = "O preço deve ser um valor positivo.")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Preço base (€)")]
        public decimal PrecoBase { get; set; }

        [Required]
        [Display(Name = "Categoria")]
        public CategoriaServico Categoria { get; set; }

        // Relação muitos-para-muitos: um Serviço pode estar em várias Cerimónias.
        public ICollection<CerimoniaServico> CerimoniaServicos { get; set; } = new List<CerimoniaServico>();
    }

    public enum CategoriaServico
    {
        [Display(Name = "Cerimónia")]
        Cerimonia,
        [Display(Name = "Transporte")]
        Transporte,
        [Display(Name = "Floral")]
        Floral,
        [Display(Name = "Documentação")]
        Documentacao,
        [Display(Name = "Outro")]
        Outro
    }
}
