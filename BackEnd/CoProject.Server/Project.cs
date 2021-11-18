namespace CoProject.Server;

public class Project
{
    public int id { get; set; }

    public string name { get; set; }

    public DateTime created { get; set; }
    
    public int supervisorID { get; set; }
    
    public int? min { get; set; }
    
    public int? max { get; set; }
    
    public int stateID { get; set; }
}