namespace CoProject.Infrastructure;

[ExcludeFromCodeCoverage]
public static class SeedExtensions
{
    public static async Task<IHost> SeedAsync(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<CoProjectContext>();
            await SeedDatabase(context);
        }

        return host;
    }

    private static async Task SeedDatabase(CoProjectContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Projects.AnyAsync())
        {
            return;
        }

        // Set up the default database
        var deniz = new User {Id = "1", Name = "Deniz", Email = "deyi@itu.dk", Supervisor = true, Image = "/images/noimage.jpeg"};
        var mikkel = new User {Id = "2", Name = "Mikkel", Email = "milb@itu.dk", Supervisor = true, Image = "/images/noimage.jpeg"};
        var danyal = new User {Id = "3", Name = "Danyal", Email = "dayo@itu.dk", Supervisor = false, Image = "/images/noimage.jpeg"};
        var jakob = new User {Id = "4", Name = "Jakob", Email = "jarh@itu.dk", Supervisor = false, Image = "/images/noimage.jpeg"};
        var lotte = new User {Id = "5", Name = "Lotte", Email = "loda@itu.dk", Supervisor = false, Image = "/images/noimage.jpeg"};

        await context.Users.AddRangeAsync(deniz, mikkel, danyal, jakob, lotte);
        await context.SaveChangesAsync();

        await context.Projects.AddRangeAsync(
            new Project
            {
                Name = "Machine Learning - Chess",
                Description =
                    "Are you looking to write your thesis about machine learning and you're a fan of chess? Look now further! " +
                    "As your supervisor, I'll help you with the technicalities, and answer any questions you may have. ",
                Created = DateTime.Now,
                SupervisorId = deniz.Id,
                Min = 2,
                Max = 5,
                Tags = await RetrieveTags(context, "Python", "Machine", "AI", "Chess"),
                Users = new List<User> {danyal, jakob}
            },
            new Project
            {
                Name = "Web Development - Human Behaviour",
                Description =
                    "I have studied human behavior, cognition and dark patterns for the past 20 years of my life. If you're interested " +
                    "in writing your thesis about how humans interact with the internet, and how big corporations tries to trick users " +
                    "into browsing their item, I could be the supervisor for your project. For any questions, please reach out!",
                Created = DateTime.Now,
                SupervisorId = mikkel.Id,
                Min = 1,
                Max = 3,
                Tags = await RetrieveTags(context, "Web", "JS", "HTML", "CSS", "UX", "UI", "Cognition", "Behavior"),
                Users = new List<User> {lotte}
            }
        );
        await context.SaveChangesAsync();
    }

    private static async Task<ICollection<Tag>> RetrieveTags(CoProjectContext context, params string[] names)
    {
        var tags = new List<Tag>();

        foreach (var name in names)
        {
            var tag = await context.Tags.FirstOrDefaultAsync(t => t.Name == name);

            if (tag == null)
            {
                tag = new() {Name = name};
                await context.Tags.AddAsync(tag);
                await context.SaveChangesAsync();
            }

            tags.Add(tag);
        }

        return tags;
    }
}