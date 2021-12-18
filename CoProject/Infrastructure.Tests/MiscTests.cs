namespace CoProject.Infrastructure.Tests;

public class MiscTests : DefaultTests
{
    [Fact]
    public void Project_ToString_returns_properly_formatted_string()
    {
        var expected = $"Id={Project.Id} \n" +
                       $"Name={Project.Name} \n" +
                       $"Description={Project.Description} \n" +
                       $"Created={Project.Created} \n" +
                       $"SupervisorId={Project.SupervisorId} \n" +
                       $"Min={Project.Min} \n" +
                       $"Max={Project.Max} \n" +
                       $"Tags={string.Join(", ", Project.Tags.Select(tag => tag.Name).ToList())} \n" +
                       $"Users={string.Join(", ", Project.Users.Select(user => user.Email).ToList())} \n" +
                       $"State={Project.State} \n";

        Assert.Equal(expected, Project.ToString());
    }
}