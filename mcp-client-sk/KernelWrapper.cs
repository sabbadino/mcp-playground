using Microsoft.SemanticKernel;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

public class KernelWrapper
{
    public required Kernel Kernel { get; init; }
    public required string Name { get; init; }

    public ImmutableList<Model> Models { get; init; } = [] ;
    public required string SystemMessageName { get; init; }
}