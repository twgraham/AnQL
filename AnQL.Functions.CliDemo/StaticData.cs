namespace AnQL.Functions.CliDemo;

public static class StaticData
{
    public static List<Car> Cars = new()
    {
        new Car { Make = "Hyundai", Model = "i20", Price = 17999.99M, NumberOfDoors = 2, InitialRelease = new DateTime(2015, 1, 1) },
        new Car { Make = "Hyundai", Model = "i30", Price = 27999.99M, NumberOfDoors = 4, InitialRelease = new DateTime(2016, 1, 1)},
        new Car { Make = "Mazda", Model = "CX-5", Price = 49999.99M, NumberOfDoors = 4, InitialRelease = new DateTime(2009, 6, 13)},
        new Car { Make = "Mazda", Model = "CX-3", Price = 29999.99M, NumberOfDoors = 4, InitialRelease = new DateTime(2010, 2, 13)},
        new Car { Make = "Polestar", Model = "2", Price = 72999.99M, NumberOfDoors = 4, InitialRelease = new DateTime(2020, 10, 12)},
        new Car { Make = "Tesla", Model = "Model 3", Price = 69999.99M, NumberOfDoors = 4, InitialRelease = new DateTime(2015, 2, 20)},
    };
}