using System.ComponentModel.DataAnnotations;

namespace funeraisefunerais.Models;

public class ContactRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório.")]
    [EmailAddress(ErrorMessage = "Insira um email válido.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Insira um telefone válido.")]
    [StringLength(20)]
    public string? Telefone { get; set; }

    [Required(ErrorMessage = "O assunto é obrigatório.")]
    [StringLength(150)]
    public string Assunto { get; set; } = string.Empty;

    [Required(ErrorMessage = "A mensagem é obrigatória.")]
    [StringLength(2000)]
    public string Mensagem { get; set; } = string.Empty;

    public DateTime DataPedido { get; set; } = DateTime.UtcNow;

    public bool Respondido { get; set; }
}
