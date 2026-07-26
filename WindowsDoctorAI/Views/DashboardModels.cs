namespace WindowsDoctorAI.Views;

public class CategoryHealthItem
{
    public string Glyph { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string StatusLabel { get; set; } = "";
    public string StatusType { get; set; } = "Healthy"; // Healthy, Warning, Critical, Info
    public double Score { get; set; } = 100;
}