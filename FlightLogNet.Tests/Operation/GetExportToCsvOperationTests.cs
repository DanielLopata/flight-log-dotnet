namespace FlightLogNet.Tests.Operation
{
    using System;
    using System.Text;

    using FlightLogNet.Operation;
    using Microsoft.Extensions.Configuration;

    using Xunit;

    public class GetExportToCsvOperationTests(GetExportToCsvOperation getExportToCsvOperation, IConfiguration configuration)
    {
        [Fact]
        public void Execute_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            TestDatabaseGenerator.DeleteOldDatabase(configuration);
            DateTime fixedDate = new(2020, 1, 2, 16, 57, 10, DateTimeKind.Local);
            TestDatabaseGenerator.CreateTestDatabaseWithFixedTime(fixedDate, configuration);

            // Act
            var result = getExportToCsvOperation.Execute();
            var csvText = Encoding.UTF8.GetString(result);

            // Assert
            Assert.NotEmpty(result);
            Assert.StartsWith("FlightId,Datum,TakeoffTime,LandingTime,Immatriculation,Type,Pilot,Copilot,Task,TowplaneID,GliderID", csvText);
            Assert.Contains("24058", csvText);
            Assert.Contains("24057", csvText);
            Assert.Contains("OK-B128", csvText);
            Assert.Contains("OK-V23428", csvText);
            Assert.Contains("Silvie Hronová", csvText);
            Assert.Contains("Petr Hrubec", csvText);
        }
    }
}
