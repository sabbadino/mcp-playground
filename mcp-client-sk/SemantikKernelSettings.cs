using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


public class SemanticKernelsSettings
{
    
    public List<KernelSettings> Kernels { get; init; } = new();
}

public class KernelSettings
{
    public string SystemMessageName { get; init; } = "";
    public bool IsDefault { get; init; } = false;   
    public string Name { get; init; }
    public List<Model> Models { get; init; } = new();
    public List<string> Plugins { get; init; } = new();

    public List<McpPlugins> McpPlugins { get; init; } = new();


}
public class McpPlugins
{
    public string Url { get; init; } = "";
    public List<string> AcceptedTools { get; init; } = new();
    public string AsSkPluginNamed { get; init; } = "";
}

public class Model
{
    public bool IsDefault { get; init; } = false;
    public ModelCategory? Category { get; init; }
  
    public string ModelName { get; init; } = "";
   
    public string Url { get; init; } = "";

    public bool UrlRequired { get; init; } = false;

    public string ApiKeyName { get; init; } = "";
    public string ServiceId { get; init; } = "";
}

public enum ModelCategory
{
    None,
    OpenAi, 
    Ollama,
    Gemini
}
