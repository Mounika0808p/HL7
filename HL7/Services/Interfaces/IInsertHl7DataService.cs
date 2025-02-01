namespace HL7.Services.Interfaces
{
    public interface IInsertHl7DataService
    {
        int InsertHl7Data(Dictionary<string, string> hl7Data);
    }
}
