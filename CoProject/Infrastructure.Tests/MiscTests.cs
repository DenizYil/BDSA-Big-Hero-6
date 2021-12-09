namespace CoProject.Infrastructure.Tests;

public class MiscTests : DefaultTests
{
    [Fact]
    public void Project_ToString_returns_properly_formatted_string()
    {
        var expected = $"Id={project.Id} \n" +
                       $"Name={project.Name} \n" +
                       $"Description={project.Description} \n" +
                       $"Created={project.Created} \n" +
                       $"SupervisorId={project.SupervisorId} \n" +
                       $"Min={project.Min} \n" +
                       $"Max={project.Max} \n" +
                       $"Tags={string.Join(", ", project.Tags.Select(tag => tag.Name).ToList())} \n" +
                       $"Users={string.Join(", ", project.Users.Select(user => user.Email).ToList())} \n" +
                       $"State={project.State} \n";

        expected.Should().BeEquivalentTo(project.ToString());
    }
}