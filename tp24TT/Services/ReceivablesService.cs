using Microsoft.EntityFrameworkCore;
using tp24TT.Data;
using tp24TT.Data.DataModels;
using tp24TT.Models.Request;
using tp24TT.Models.Response;

namespace tp24TT.Services
{

    public interface IReceivablesService
    {
        Task<PayloadResponse> SendPayload(ReceivableRequest request);
        Task<ReceivablesSummaryResponse> GetReceivablesSummary();
    }

    public class ReceivablesService : IReceivablesService
    {
        private readonly Tp24ttContext _Context;
        public ReceivablesService(Tp24ttContext Context)
        {
            _Context = Context;
        }
        public async Task<PayloadResponse> SendPayload(ReceivableRequest request)
        {
            try
            {
                var response = new PayloadResponse() { request = request, Errors = new List<string>()};

                DateTime issueDate;
                DateTime dueDate;
                DateTime? closedDate = null; // Use nullable DateTime for optional ClosedDate
                

                var data = await _Context.Receivables.FirstOrDefaultAsync(x => x.Reference == request.Reference);

                if (data != null)
                {
                    response.Errors.Add($"Error - Record with reference {request.Reference} already exists");
                }   

                // Attempt to parse the date strings to DateTime format
                if (!DateTime.TryParse(request.IssueDate, out issueDate))
                {
                    response.Errors.Add($"Error - Invalid Date format, unable to parse issueDate {request.IssueDate}");

                }

                if (!DateTime.TryParse(request.DueDate, out dueDate))
                {
                    response.Errors.Add($"Error - Invalid Date format, unable to parse dueDate {request.DueDate}");
                }

                // Parse ClosedDate if it's provided
                if (!string.IsNullOrEmpty(request.ClosedDate))
                {
                    if (DateTime.TryParse(request.ClosedDate, out DateTime parsedClosedDate))
                    {
                        closedDate = parsedClosedDate;

                        // Check if ClosedDate is before IssueDate or after DueDate
                        if (closedDate < issueDate)
                        {
                            response.Errors.Add($"Error - closedDate cannot be before issueDate");
                        }

                        if (closedDate > dueDate)
                        {
                            response.Errors.Add($"Error - closedDate cannot be after dueDate");
                        }
                    }
                    else
                    {
                        response.Errors.Add($"Error - Invalid Date format, unable to parse closedDate {request.ClosedDate}");
                    }
                }

                decimal openingValue = request.OpeningValue;
                decimal paidValue = request.PaidValue;

                bool cancelled = request.Cancelled ?? false;

                if (cancelled && closedDate == null)
                {
                    response.Errors.Add($"Error - Cannot cancel without a closedDate, Please provide a closedDate");
                }

                //the assumption here is that that the account cannot be closed with a paid value less than the opening value, unless it has been cancelled
                // Check if PaidValue is greater than or equal to OpeningValue when ClosedDate is not null and Cancelled is false or null
                if (closedDate != null && !cancelled && paidValue < openingValue)
                {
                   
                    response.Errors.Add($"Error - unable to closed account, cancelled cannot be false if paidValue: {paidValue} is not equal or greater to openingValue: {openingValue}  ");
                }

                if (!response.Errors.Any())
                {
                    var receivable = new Receivable
                    {
                        Reference = request.Reference,
                        CurrencyCode = request.CurrencyCode,
                        IssueDate = issueDate,
                        OpeningValue = openingValue,
                        PaidValue = paidValue,
                        DueDate = dueDate,
                        ClosedDate = closedDate,
                        Cancelled = cancelled,
                        DebtorName = request.DebtorName,
                        DebtorReference = request.DebtorReference,
                        DebtorAddress1 = request.DebtorAddress1,
                        DebtorAddress2 = request.DebtorAddress2,
                        DebtorTown = request.DebtorTown,
                        DebtorState = request.DebtorState,
                        DebtorZip = request.DebtorZip,
                        DebtorCountryCode = request.DebtorCountryCode,
                        DebtorRegistrationNumber = request.DebtorRegistrationNumber
                    };

                    // Add the new Receivable object to the DbContext
                    await _Context.Receivables.AddAsync(receivable);

                    // Save changes to the database
                    await _Context.SaveChangesAsync();
                }

                return response;

            }
            catch (Exception ex)
            {
                // Handle any exceptions or errors here
                var response = new PayloadResponse() { request = request, Errors = new List<string> { $"Error - {ex.Message} request failed, please contact helpdesk....or whoever" } };
                return response;
            }
        }




        public async Task<ReceivablesSummaryResponse>  GetReceivablesSummary()
        {
            List<Receivable> receivables = await _Context.Receivables.ToListAsync();

            int totalEntries = receivables.Count;
            int openEntries = receivables.Count(r => r.ClosedDate == null);
            int closedEntries = receivables.Count(r => r.ClosedDate.HasValue);
            int cancelledEntries = receivables.Count(r => r.Cancelled == true);

            decimal totalEntriesDecimal = totalEntries;

            decimal percentageOpen = openEntries / totalEntriesDecimal * 100M;
            decimal percentageClosed = closedEntries / totalEntriesDecimal * 100M;
            decimal percentageCancelled = cancelledEntries / totalEntriesDecimal * 100M;

            decimal totalPaidValueOpen = receivables
                .Where(r => r.ClosedDate == null || r.IssueDate > DateTime.Now)
                .Sum(r => r.PaidValue);

            decimal totalPaidValueClosed = receivables
                .Where(r => r.ClosedDate.HasValue)
                .Sum(r => r.PaidValue);

            var summary = new ReceivablesSummaryResponse
            {
                TotalEntries = totalEntries,
                PercentageOpen = percentageOpen,
                PercentageClosed = percentageClosed,
                PercentageCancelled = percentageCancelled,
                TotalPaidValueOpen = totalPaidValueOpen,
                TotalPaidValueClosed = totalPaidValueClosed
            };

            return summary;
        }

    }
}
