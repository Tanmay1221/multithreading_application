using Newtonsoft.Json;
namespace multithreading_async_await;

class Program
{
    private static Queue<string> readBlobs = new Queue<string>();
    private static object lockObject = new object();

    static async Task Main(string[] args)
    {
        string folderPath = "/Users/tanmayfuse/Projects/tenant";

        Task readFileTask = ReadfileAsync(folderPath);
        Task processTask = ProcessBlobAsync();

        await Task.WhenAll(readFileTask, processTask);

        Console.WriteLine("Program completed");
    }

    static async Task ReadfileAsync(string folderPath)
    {
        try
        {
            string[] files = Directory.GetFiles(folderPath,"*.json").OrderBy(file => Path.GetFileName(file)).ToArray(); ;
            foreach(string file in files)
            {
                string fileContent = await ReadFileContentAsync(file);
                lock(lockObject)
                {
                    readBlobs.Enqueue(fileContent);
                }
                Console.WriteLine($"Read Blob from {file}");

                await Task.Delay(500);
            }
        } catch (Exception ex)
        {
            Console.WriteLine($"Error reading files : {ex.Message}");
        }
    }

    static async Task ProcessBlobAsync()
    {
        int blobProcessed = 0;

        while(blobProcessed < 100)
        {
            lock(lockObject)
            {
                if (readBlobs.Count >= 40 || blobProcessed > 40)
                {
                    string blobData = readBlobs.Dequeue();
                    dynamic jsonData = JsonConvert.DeserializeObject(blobData);
                    string baseUrl = jsonData.baseurl;
                    Console.WriteLine($"Base URL : {baseUrl}");
                    foreach (var relativeUrl in jsonData.relativeurl)
                    {
                        string url = relativeUrl.url;
                        Console.WriteLine($"Relative URL is {url}");
                    }

                    blobProcessed++;
                }
            }
        }
    }

    static async Task<string> ReadFileContentAsync(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            return await reader.ReadToEndAsync();
        }
    }
}

