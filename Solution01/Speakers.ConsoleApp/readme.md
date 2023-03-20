
``` cs
var serverProvider = new SqlSyncChangeTrackingProvider(GetDevdayBeConnectionString());
var clientProvider = new SqliteSyncProvider("devdaybe.db");

var setup = new SyncSetup("Speakers");

```

``` cs
do {
    try {

    }
    catch (Exception ex) {
        Console.WriteLine(ex);
    }

} while (Console.ReadKey().Key != ConsoleKey.Escape);
```


``` cs
var progress = new SynchronousProgress<ProgressArgs>(s =>
    Console.WriteLine($"{s.ProgressPercentage:p}:  \t[{s?.Source[..Math.Min(4, s.Source.Length)]}] {s.TypeName}: {s.Message}"));

```


``` cs

var agent = new SyncAgent(clientProvider, serverProvider);

```

``` cs
var syncResult = await agent.SynchronizeAsync(setup, progress);
Console.WriteLine(syncResult);

```
            