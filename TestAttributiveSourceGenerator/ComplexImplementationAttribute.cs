namespace TestAttributiveSourceGenerator;

[AttributeUsage(AttributeTargets.Class)]
public class ComplexImplementationAttribute : Attribute
{
    public required string[] Usings { get; set; }
    public required string Pattern { get; set; }
}
