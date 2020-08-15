namespace ProjectV.DataAccessLayer.Orm
{
    internal interface IMapper<TFirst, TSecond>
    {
        TFirst Map(TSecond model);

        TSecond Map(TFirst model);
    }
}
