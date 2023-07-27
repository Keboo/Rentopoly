using Microsoft.EntityFrameworkCore;

using Rentopoly.Data;
using Rentopoly.Rentals;

namespace Rentopoly.Tests.Rentals;

public class AddRentalViewModelTests
{
    [Fact]
    public async Task OnSubmit_WithValidData_AddNewRentalToDatabase()
    {
        // Arrange
        AutoMocker mocker = new();
        using var factory = mocker.WithDbScope();

        AddRentalViewModel vm = mocker.CreateInstance<AddRentalViewModel>();

        // Act
        vm.LoanedTo = "Test Loner";
        await vm.SubmitCommand.ExecuteAsync(null);

        // Assert
        using var assertContext = factory.CreateDbContext();
        Rental rental = await assertContext.Rentals.SingleAsync();
        Assert.Equal("Test Loner", rental.LoanedTo);
        Assert.Equal(DateTime.Today, rental.LoanedOn);
        Assert.Null(rental.ReturnedOn);
    }
}
