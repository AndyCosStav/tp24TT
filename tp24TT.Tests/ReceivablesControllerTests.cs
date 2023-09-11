using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tp24TT.Controllers;
using tp24TT.Data.DataModels;
using tp24TT.Data;
using tp24TT.Models.Request;
using tp24TT.Services;
using Microsoft.EntityFrameworkCore;
using tp24TT.Models.Response;

namespace tp24TT.Tests
{
    public class ReceivablesControllerTests
    {
        [Fact]
        public async Task Payload_ValidRequest_ReturnsCreated()
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

            var mockService = new Mock<IReceivablesService>();
            mockService.Setup(service => service.SendPayload(It.IsAny<ReceivableRequest>()))
                .ReturnsAsync(new PayloadResponse { Errors = new List<string>() });

            var controller = new ReceivablesController(mockService.Object);

            // Act
            var result = await controller.Payload(request);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.IsType<PayloadResponse>(createdResult.Value);
        }

        [Fact]
        public async Task Payload_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new ReceivableRequest
            {
                // empty request to signify dud 
            };

            var mockService = new Mock<IReceivablesService>();
            mockService.Setup(service => service.SendPayload(It.IsAny<ReceivableRequest>()))
                .ReturnsAsync(new PayloadResponse { Errors = new System.Collections.Generic.List<string> { "Error message" } });

            var controller = new ReceivablesController(mockService.Object);

            // Act
            var result = await controller.Payload(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.IsType<PayloadResponse>(badRequestResult.Value);
        }
    }

}

