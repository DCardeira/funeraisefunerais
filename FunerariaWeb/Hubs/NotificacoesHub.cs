using Microsoft.AspNetCore.SignalR;

namespace FunerariaWeb.Hubs
{
    // Hub SignalR: usado para avisar em tempo real o painel de administração
    // sempre que uma Cerimonia é criada, editada ou apagada, sem precisar
    // de recarregar a página (satisfaz o requisito de uso de SignalR).
    public class NotificacoesHub : Hub
    {
        public const string RotaHub = "/hubs/notificacoes";

        public async Task EntrarNoGrupoAdmins()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Administradores");
        }
    }

    // Serviço auxiliar injetado nas Razor Pages / Controllers para disparar notificações
    // sem que cada página precise de conhecer os detalhes do SignalR.
    public class NotificadorCerimonias
    {
        private readonly IHubContext<NotificacoesHub> _hub;

        public NotificadorCerimonias(IHubContext<NotificacoesHub> hub)
        {
            _hub = hub;
        }

        public Task AvisarAlteracaoAsync(string mensagem)
        {
            return _hub.Clients.Group("Administradores").SendAsync("NovaNotificacao", mensagem, DateTime.Now.ToString("HH:mm:ss"));
        }
    }
}
