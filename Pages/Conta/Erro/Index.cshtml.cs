using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FunerariaWeb.Pages.Erro
{
    public class IndexModel : PageModel
    {
        public string Titulo { get; set; } = "Ocorreu um erro";
        public string Mensagem { get; set; } = "Por favor tente novamente mais tarde.";

        public void OnGet(int? codigo)
        {
            (Titulo, Mensagem) = codigo switch
            {
                404 => ("Página não encontrada", "A página que procura não existe ou foi movida."),
                403 => ("Acesso negado", "Não tem permissões para aceder a este recurso."),
                500 => ("Erro no servidor", "Ocorreu um problema inesperado, possivelmente no acesso à base de dados. A equipa foi notificada."),
                _ => ("Ocorreu um erro", "Por favor tente novamente mais tarde.")
            };
        }
    }
}
