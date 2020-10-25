namespace middler.Common.SharedModels.Interfaces
{
    public interface IMiddlerAction
    {
        bool Terminating { get; set; }
        bool WriteStreamDirect { get; set; }
        bool Enabled { get; set; }
        string ActionType { get; }

    }


}