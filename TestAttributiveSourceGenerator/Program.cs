using TestAttributiveSourceGenerator.Attributes;

namespace TestAttributiveSourceGenerator;

public partial class Program
{
    static void Main(string[] args)
    {
        new Program().HelloFromGenerator();
    }

    [HelloImplementation("Hello from the other side~")]
    public partial void HelloFromGenerator();
}
