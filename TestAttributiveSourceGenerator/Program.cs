using TestAttributiveSourceGenerator.Attributes;

namespace TestAttributiveSourceGenerator;

public partial class Program
{
    static void Main(string[] args)
    {
        new Program().HelloFromGenerator();
    }

    [HelloImplementation("Fuck.")]
    public partial void HelloFromGenerator();
}
