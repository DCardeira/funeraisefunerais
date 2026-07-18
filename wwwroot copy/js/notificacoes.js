// Liga-se ao NotificacoesHub e mostra um "toast" sempre que uma Cerimonia
// é criada/editada/apagada, para qualquer Administrador ligado no momento.
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/notificacoes")
    .withAutomaticReconnect()
    .build();

connection.on("NovaNotificacao", (mensagem, hora) => {
    const caixa = document.getElementById("notificacoes-caixa");
    if (!caixa) return;

    const toast = document.createElement("div");
    toast.className = "toast-notificacao";
    toast.innerHTML = `<strong>${hora}</strong> — ${mensagem}`;
    caixa.appendChild(toast);

    setTimeout(() => toast.remove(), 6000);
});

connection.start()
    .then(() => connection.invoke("EntrarNoGrupoAdmins"))
    .catch(err => console.error("Erro ao ligar ao SignalR:", err));
