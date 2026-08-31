namespace GatchTycoon.Domain
{
    public enum BuildingCategory
    {
        CityHall,
        Residence,
        Work,
        Convenience,
        Public,
        Special
    }

    public enum CurrencyType
    {
        Gold,
        Town2Currency
    }
    
    public enum BuffType
    {
        OccupancyRate,
        MoneyEfficiency
    }
    
    public enum RangePattern
    {
        None,
        LeftRight,
        Cross,
        Square3x3,
        TopDiagonals,
        AllDiagonals
    }
}
