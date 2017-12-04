﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace GenerareDateApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Presiune")]
    public class PresiuneController : Controller
    {
        // GET: api/Presiune
        [HttpGet]
        public IEnumerable<string> GetPres()
        {
            List<string> listaSenzori = new List<string>();
            string message = "";
            string url = "";
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(url.Replace("amqp://", "amqps://"));

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "queuePresiune", durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume("queuePresiune", false, consumer);
                    BasicDeliverEventArgs ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                    var body = ea.Body;
                    message = Encoding.UTF8.GetString(body);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            }
            listaSenzori.Add(message);
            return listaSenzori.ToArray();
        }
    }
}
