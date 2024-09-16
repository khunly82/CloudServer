using Microsoft.AspNetCore.SignalR;

namespace CloudServer.Hubs
{
    public class IOTHub : Hub
    {
        public void Send(IotInfoDTO dto)
        {
            // enregistrer les message en db ...
            Clients.Others.SendAsync(dto.Topic, dto.Payload);
        }
    }

    public class IotInfoDTO
    {
        public string Topic { get; set; } = null!;
        public string Payload { get; set; } = null!;
    }
}
