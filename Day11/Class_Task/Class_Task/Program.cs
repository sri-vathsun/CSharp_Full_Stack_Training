
public class Program
{
    public static async Task Main(string[] args)
    {
        LearnAsyncProgramming learning = new LearnAsyncProgramming();

        await learning.BasicAsync();

        await learning.TrackTaskAsync();

        await learning.TaskTrackingAsync();

        await learning.SequentialAsync();

        Console.WriteLine("Main method completed");
    }
}
