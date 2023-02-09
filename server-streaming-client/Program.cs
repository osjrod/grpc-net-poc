using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using server_streaming_client;

var channel = GrpcChannel.ForAddress("https://localhost:7069");

var client = new Poc.PocClient(channel);
using (var stream = client.Receive(new Empty()))
{

    var readTask = Task.Run(async () =>
    {
        await foreach (var response in stream.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(response.Value);
        }
    });

    
    await readTask;
}
Console.ReadKey();