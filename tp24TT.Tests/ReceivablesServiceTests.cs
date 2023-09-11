using Microsoft.EntityFrameworkCore;
using Moq;
using tp24TT.Data;
using tp24TT.Data.DataModels;
using tp24TT.Models.Request;
using tp24TT.Services;

namespace tp24TT.Tests
{
    public class ReceivablesServiceTests
    {

        [Fact]
        public async Task SendPayload_SuccessfulPayload_ReturnsPayloadResponseWithNoErrors()
        {
            // Arrange
            var request = new ReceivableRequest
            {
                Reference = Guid.NewGuid(),
                CurrencyCode = "USD",
                IssueDate = "27/08/2019",
                OpeningValue = 1000.00m,
                PaidValue = 0.00m,
                DueDate = "27/08/2030",
                ClosedDate = null,
                Cancelled = false,
                DebtorName = "John Doe",
                DebtorReference = "JD123",
                DebtorAddress1 = "123 Main St",
                DebtorAddress2 = "Apt 4B",
                DebtorTown = "Springfield",
                DebtorState = "IL",
                DebtorZip = "12345",
                DebtorCountryCode = "US",
                DebtorRegistrationNumber = "987654321",
            };

            var options = new DbContextOptionsBuilder<Tp24ttContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            using (var context = new Tp24ttContext(options))
            {
                var service = new ReceivablesService(context);

                // Act
                var response = await service.SendPayload(request);

                // Assert
                Assert.NotNull(response);
                Assert.Empty(response.Errors); 
            }
        }


        [Fact]
        public async Task SendPayload_InvalidDateFormats_ReturnsPayloadResponseWithErrors()
        {
            // Arrange
            var request = new ReceivableRequest
            {
                Reference = Guid.NewGuid(),
                CurrencyCode = "USD",
                IssueDate = "3 years ago",
                OpeningValue = 1000.00m,
                PaidValue = 0.00m,
                DueDate = "trying to break things",
                ClosedDate = null,
                Cancelled = false,
                DebtorName = "John Doe",
                DebtorReference = "JD123",
                DebtorAddress1 = "123 Main St",
                DebtorAddress2 = "Apt 4B",
                DebtorTown = "Springfield",
                DebtorState = "IL",
                DebtorZip = "12345",
                DebtorCountryCode = "US",
                DebtorRegistrationNumber = "987654321",
            };

            var options = new DbContextOptionsBuilder<Tp24ttContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new Tp24ttContext(options))
            {
                var service = new ReceivablesService(context);

                // Act
                var response = await service.SendPayload(request);

                // Assert
                Assert.NotNull(response);
                Assert.NotEmpty(response.Errors);

                // Assert specific error messages for date parsing
                Assert.Contains($"Error - Invalid Date format, unable to parse issueDate {request.IssueDate}", response.Errors);
                Assert.Contains($"Error - Invalid Date format, unable to parse dueDate {request.DueDate}", response.Errors);
            }
        }


        [Fact]
        public async Task SendPayload_CancelledWithoutClosedDate_ReturnsPayloadResponseWithErrors()
        {
            // Arrange
            var request = new ReceivableRequest
            {
                Reference = Guid.NewGuid(),
                CurrencyCode = "USD",
                IssueDate = "27/08/2019",
                OpeningValue = 1000.00m,
                PaidValue = 0.00m,
                DueDate = "27/08/2030",
                ClosedDate = null,
                Cancelled = true,
                DebtorName = "John Doe",
                DebtorReference = "JD123",
                DebtorAddress1 = "123 Main St",
                DebtorAddress2 = "Apt 4B",
                DebtorTown = "Springfield",
                DebtorState = "IL",
                DebtorZip = "12345",
                DebtorCountryCode = "US",
                DebtorRegistrationNumber = "987654321",
            };

            var options = new DbContextOptionsBuilder<Tp24ttContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new Tp24ttContext(options))
            {
                var service = new ReceivablesService(context);

                // Act
                var response = await service.SendPayload(request);

                // Assert
                Assert.NotNull(response);
                Assert.NotEmpty(response.Errors);

                // Assert the specific error message for cancelling without a closedDate
                Assert.Contains($"Error - Cannot cancel without a closedDate, Please provide a closedDate", response.Errors);
            }
        }

        [Fact]
        public async Task GetReceivablesSummary_ReturnsSummaryResponse()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<Tp24ttContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new Tp24ttContext(options))
            {
                
                context.Receivables.Add(new Receivable {
                    Reference = Guid.NewGuid(),
                    CurrencyCode = "USD",
                    IssueDate = DateTime.Parse("27/08/2019"),
                    OpeningValue = 1000.00m,
                    PaidValue = 0.00m,
                    DueDate = DateTime.Parse("27/08/2030"),
                    ClosedDate = null,
                    Cancelled = false,
                    DebtorName = "John Doe",
                    DebtorReference = "JD123",
                    DebtorAddress1 = "123 Main St",
                    DebtorAddress2 = "Apt 4B",
                    DebtorTown = "Springfield",
                    DebtorState = "IL",
                    DebtorZip = "12345",
                    DebtorCountryCode = "US",
                    DebtorRegistrationNumber = "987654321",
                });
                context.Receivables.Add(new Receivable
                {
                    Reference = Guid.NewGuid(),
                    CurrencyCode = "EUR",
                    IssueDate = DateTime.Parse("27/08/2017"),
                    OpeningValue = 10000.00m,
                    PaidValue = 10000.00m,
                    DueDate = DateTime.Parse("27/08/2035"),
                    ClosedDate = DateTime.Parse("27/08/2032"),
                    Cancelled = false,
                    DebtorName = "John Doe",
                    DebtorReference = "JD123",
                    DebtorAddress1 = "123 Main St",
                    DebtorAddress2 = "Apt 4B",
                    DebtorTown = "Springfield",
                    DebtorState = "IL",
                    DebtorZip = "12345",
                    DebtorCountryCode = "US",
                    DebtorRegistrationNumber = "987654321",
                });
                // Add more sample data as needed

                context.SaveChanges();

                var service = new ReceivablesService(context);

                // Act
                var summary = await service.GetReceivablesSummary();

                // Assert
                Assert.NotNull(summary);
                Assert.Equal(2, summary.TotalEntries);
            }
        }
    }
}