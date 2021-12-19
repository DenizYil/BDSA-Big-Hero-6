namespace CoProject.Shared;

public class UploadedFile
{
    public string FileName { get; set; }
    public byte[] FileContent { get; set; }
}

public class UpdateUserBody
{
    public UserUpdateDTO updatedUser { get; set; }
    public UploadedFile? file { get; set; }
}