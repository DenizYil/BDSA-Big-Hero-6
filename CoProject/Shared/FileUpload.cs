namespace CoProject.Shared;

public record FileUpload(string Name, byte[] Content)
{
    public string? Path { get; set; }
}