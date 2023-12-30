namespace TestAttributiveSourceGenerator;

public partial class Program
{
    static void Main(string[] args)
    {
        new Program().HelloFromGenerator();
        Console.WriteLine(
            new MyComplexImplementation()
            .StringBuilder
            .Append("The very generator project needs Roslyn dependencies, others dont")
            .ToString());
    }

    [HelloImplementation("Generator: Hello from the other side~")]
    public partial void HelloFromGenerator();
}
