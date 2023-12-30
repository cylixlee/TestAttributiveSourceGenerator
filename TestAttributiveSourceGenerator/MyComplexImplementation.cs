namespace TestAttributiveSourceGenerator;

[ComplexImplementation(
    Usings = ["System.Text"],
    Pattern = "public StringBuilder StringBuilder { get ;} = new();"
)]
public partial class MyComplexImplementation;
