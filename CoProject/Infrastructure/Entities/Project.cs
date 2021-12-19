namespace CoProject.Infrastructure.Entities;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Created { get; set; }
    public string SupervisorId { get; set; } = null!;
    public int? Min { get; set; }
    public int? Max { get; set; }
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public ICollection<User> Users { get; set; } = new List<User>();
    public State State { get; set; }

    public override string ToString()
    {
        var tags = string.Join(", ", Tags.Select(tag => tag.Name).ToList());
        var users = string.Join(", ", Users.Select(user => user.Email).ToList());

        return $"Id={Id} \n" +
               $"Name={Name} \n" +
               $"Description={Description} \n" +
               $"Created={Created} \n" +
               $"SupervisorId={SupervisorId} \n" +
               $"Min={Min} \n" +
               $"Max={Max} \n" +
               $"Tags={tags} \n" +
               $"Users={users} \n" +
               $"State={State} \n";
    }
}