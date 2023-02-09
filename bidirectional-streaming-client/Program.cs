using bidirectional_streaming_client;
using Grpc.Core;
using Grpc.Net.Client;

var channel = GrpcChannel.ForAddress("https://localhost:7069");

var client = new Poc.PocClient(channel);
using (var stream = client.Guess())
{
    var found = false;

    var readTask = Task.Run(async () =>
    {
        await foreach (var response in stream.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(response.Message);
            if (response.Found)
            {
                found = true;
            }
        }
        Console.WriteLine("Game finished, enter to exit");
    });

    while (!found)
    {
        Console.Write("> ");
        var ns = Console.ReadLine();
        if (!found && int.TryParse(ns, out var number))
        {
            await stream.RequestStream.WriteAsync(new Number { Value = number });
        }
    }

    await stream.RequestStream.CompleteAsync();
    await readTask;
}
Console.WriteLine("Press a key to close");
Console.ReadKey();