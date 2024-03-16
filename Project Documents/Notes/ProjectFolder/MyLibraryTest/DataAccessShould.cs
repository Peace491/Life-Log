namespace MyLibraryTest;

using System.Diagnostics;

// TDD
// Unit testing - You define a unit of work and test that

using MyLibrary;

public class DataAccessShould
{
    //ClassName_MethodName_Scenario
    [Fact]
    public void DataAccessShould_CreateANewRecordInDataStore()
    {
        // Triple A
        // RGR - Red Green Refactor

        // Arrange: Set up before test execute
        var timer = new Stopwatch();
        var dao = new DataAccessObject();
        var firstName = "Phong";
        var lastName = "Cao";

        // Act
        timer.Start();
        var result = dao.CreateUser(firstName, lastName);
        timer.Stop();

        // Assert
        Assert.True(result.HasError == false);
        Assert.True(timer.Elapsed.TotalSeconds <= 3);
        // Assert data record exist in data store

    }
}