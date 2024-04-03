using Newtonsoft.Json;

public class Employee
{
    [JsonProperty("Id")]
    public string? Id { get; set; }
    
    [JsonProperty("EmployeeName")]
    public string? Name { get; set; }
    
    [JsonProperty("StarTimeUtc")]
    public DateTime StartTime { get; set; }
    
    [JsonProperty("EndTimeUtc")]
    public DateTime EndTime { get; set; }
    
    [JsonProperty("EntryNotes")]
    public string? EntryNotes { get; set; }
    
    [JsonProperty("DeletedOn")]
    public string? DeletedOn { get; set; }

}
