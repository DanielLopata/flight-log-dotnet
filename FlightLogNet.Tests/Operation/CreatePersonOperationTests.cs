namespace FlightLogNet.Tests.Operation
{
    using Integration;
    using Models;
    using FlightLogNet.Operation;
    using FlightLogNet.Repositories.Interfaces;

    using Moq;

    using Xunit;

    public class CreatePersonOperationTests
    {
        private readonly MockRepository mockRepository;

        private readonly Mock<IPersonRepository> mockPersonRepository;
        private readonly Mock<IClubUserDatabase> mockClubUserDatabase;

        public CreatePersonOperationTests()
        {
            this.mockRepository = new(MockBehavior.Strict);

            this.mockPersonRepository = this.mockRepository.Create<IPersonRepository>();
            this.mockClubUserDatabase = this.mockRepository.Create<IClubUserDatabase>();
        }

        private CreatePersonOperation CreateCreatePersonOperation()
        {
            return new(
                this.mockPersonRepository.Object,
                this.mockClubUserDatabase.Object);
        }

        [Fact]
        public void Execute_ShouldReturnNull()
        {
            // Arrange
            var createPersonOperation = this.CreateCreatePersonOperation();

            // Act
            var result = createPersonOperation.Execute(null);

            // Assert
            Assert.Null(result);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void Execute_ShouldCreateGuest()
        {
            // Arrange
            var createPersonOperation = this.CreateCreatePersonOperation();
            PersonModel personModel = new PersonModel
            {
                MemberId = 0,
                Address = new() { City = "NY", PostalCode = "456", Street = "2nd Ev", Country = "USA" },
                FirstName = "John",
                LastName = "Smith"
            };
            this.mockPersonRepository.Setup(repository => repository.AddGuestPerson(personModel)).Returns(10);

            // Act
            var result = createPersonOperation.Execute(personModel);

            // Assert
            Assert.True(result > 0);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void Execute_ShouldReturnExistingClubMember()
        {
            // Arrange
            var createPersonOperation = this.CreateCreatePersonOperation();
            PersonModel personModel = new PersonModel
            {
                FirstName = "Jan",
                LastName = "Novák",
                MemberId = 3
            };
            long id = 333;
            this.mockPersonRepository.Setup(repository => repository.TryGetPerson(personModel, out id)).Returns(true);

            // Act
            var result = createPersonOperation.Execute(personModel);

            // Assert
            Assert.Equal(id, result);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void Execute_ShouldCreateNewClubMember()
        {
            // Arrange
            var createPersonOperation = this.CreateCreatePersonOperation();
            PersonModel personModel = new PersonModel
            {
                FirstName = "Karl",
                LastName = "Lucemnburský",
                MemberId = 444
            };
            PersonModel clubUser = new PersonModel
            {
                FirstName = "Karel",
                LastName = "Lucemburský",
                MemberId = 444
            };

            long id = 0;
            this.mockPersonRepository.Setup(repository => repository.TryGetPerson(personModel, out id)).Returns(false);
            this.mockClubUserDatabase.Setup(repository => repository.TryGetClubUser(444, out clubUser)).Returns(true);
            this.mockPersonRepository.Setup(repository => repository.CreateClubMember(clubUser)).Returns(4);

            // Act
            var result = createPersonOperation.Execute(personModel);

            // Assert
            Assert.Equal(4, result);
            this.mockRepository.VerifyAll();
        }
    }
}
