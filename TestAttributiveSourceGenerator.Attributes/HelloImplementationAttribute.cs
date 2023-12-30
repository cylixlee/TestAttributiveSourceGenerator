﻿using System;

namespace TestAttributiveSourceGenerator.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class HelloImplementationAttribute(string helloContent) : Attribute
{
    public string HelloContent { get; set; } = helloContent;
}
