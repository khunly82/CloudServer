using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace CloudServer.Infrastructure
{
    public class MqttConnection
    {
        public class Configuration
        {
            public string Host { get; set; } = string.Empty;
            public int Port { get; set; }
        }

        private readonly IMqttClient _client;

        public MqttConnection(Configuration configuration)
        {
            // initilisation de la connection au broker Mqtt (Mosquitto)
            MqttFactory factory = new MqttFactory();

            // definir le serveur et le port sur lequel je veux me connecter
            MqttClientOptions options = factory.CreateClientOptionsBuilder()
                .WithTcpServer(configuration.Host, configuration.Port)
                .Build();

            _client = factory.CreateMqttClient();
            _client.ConnectAsync(options).Wait();

            var optionsBuilder = factory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter("test");

            _client.SubscribeAsync(optionsBuilder.Build()).Wait();

            // gestion de la reconnection au serveur
            _client.DisconnectedAsync += async (args) =>
            {
                await _client.ReconnectAsync();
                Thread.Sleep(2000);
            };
            
        }

        public void SubscribeAsync(string topic, Action<string> callBack)
        {
            _client.ApplicationMessageReceivedAsync += async (e) =>
            {
                if(e.ApplicationMessage.Topic == topic)
                {
                    callBack?.Invoke(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
                }
                await Task.CompletedTask;
            };
        }

        public async Task<bool> PublishAsync(string topic, string payload)
        {
            // transformation de mes données en json
            MqttClientPublishResult result = await _client.PublishStringAsync(topic, payload);
            if(result.IsSuccess)
            {
                return true;
            }
            return false;
        }
    }
}
