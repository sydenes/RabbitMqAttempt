using Microsoft.AspNetCore.SignalR;

namespace RabbitMq.ExcelCreate.Hubs
{
    public class ExcelHub:Hub
    {
        //because of we will use one direction hub, we don't need to override OnConnectedAsync method. (server --hub--> client)
        //defined in program.cs
    }
}
