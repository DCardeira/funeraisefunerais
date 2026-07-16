using System.ComponentModel.DataAnnotations;

namespace FunerariaWeb.Models
{
    // Representa a família/pessoa de contacto responsável por um processo funerário.
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(120, ErrorMessage = "O nome não pode ter mais de 120 caracteres.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Introduza um email válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone(ErrorMessage = "Introduza um número de telefone válido.")]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Morada { get; set; }

        // Liga opcionalmente este Cliente a uma conta de login (Identity),
        // para que um utilizador com papel "Cliente" só veja os seus próprios processos.
        public string? IdentityUserId { get; set; }

        // Relação muitos-para-um: muitas Cerimónias pertencem a um Cliente.
        public ICollection<Cerimonia> Cerimonias { get; set; } = new List<Cerimonia>();
    }
}
