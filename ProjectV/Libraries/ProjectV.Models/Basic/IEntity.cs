namespace ProjectV.Models.Basic
{
    public interface IEntity<TId>
        where TId : struct
    {
        TId Id { get; }
    }
}
