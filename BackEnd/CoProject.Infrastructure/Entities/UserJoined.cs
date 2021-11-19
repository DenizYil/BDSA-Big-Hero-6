namespace CoProject.Infrastructure.Entities;

public class UserJoined
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
}