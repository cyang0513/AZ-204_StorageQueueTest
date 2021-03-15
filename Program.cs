using System;
using System.Threading.Tasks;
using Azure.Storage.Queues;

namespace StorageQueueTest
{
   class Program
   {
      static async Task Main(string[] args)
      {

         var connStr = "DefaultEndpointsProtocol=https;AccountName=chyastorage;AccountKey=uzMzvbJTKwrvFlwcnRQJrFUal6D4tz2YIgqJvFCDurFJuiGxGr/OHJtPdgbdkXjIa3O8YWKb2yzotRHfJGlQig==;EndpointSuffix=core.windows.net";
         var queueName = "chyaqueue";

         Console.WriteLine("Storage Queue test...");

         var client = new QueueClient(connStr, queueName);

         client.CreateIfNotExists();

         Console.WriteLine($"Message in queue: {client.GetProperties().Value.ApproximateMessagesCount}");

         Console.WriteLine("Type messages, ENTER to send (Q to quit)...");
         while (true)
         {
            var msg = Console.ReadLine();
            if (msg == "Q")
            {
               break;
            }

            await client.SendMessageAsync(msg);
         }

         Console.WriteLine($"Message in queue: {client.GetProperties().Value.ApproximateMessagesCount}");

         Console.ReadKey();

         while (true)
         {
            var count = client.GetProperties().Value.ApproximateMessagesCount;
            if (count > 0)
            {
               var resp = await client.ReceiveMessageAsync();

               if (resp.Value == null)
               {
                  break;
               }
               Console.WriteLine($"Message: {resp.Value.MessageText}, count {count}");

               //delete from queue after process
               client.DeleteMessage(resp.Value.MessageId, resp.Value.PopReceipt);
            }
            else
            {
               break;
            }

         }

         Console.ReadKey();

      }
   }
}
