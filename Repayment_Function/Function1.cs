using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace Repayment_Function
{
    public class Function1
    {
        private ILoanOfferRepository loanOfferRepository;
        public Function1(ILoanOfferRepository loanOfferRepository)
        {
            this.loanOfferRepository = loanOfferRepository;

        }
        [FunctionName("Function1")]
        public  async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
           

            // Step 1: Retrieve all loans from the database
            var loans = await GetLoansAsync();

            // Step 2: Check if the due date of any loan has elapsed
            foreach (var loan in loans)
            {
               // Calculate the due date based on CreatedAt and LoanDurationDays
                var dueDate = loan.CreatedAt.AddDays(loan.LoanDurationDays);
                if (dueDate <= DateTime.UtcNow && !loan.Active)
                {
                    // Step 3: Debit the borrower's account
                    bool debitSuccess = await DebitBorrowerAccountAsync(loan.UserId, loan.Amount);

                    if (debitSuccess)
                    {
                        // Step 4: Mark loan as paid in the database
                        await MarkLoanAsPaidAsync(loan.Id);
                       
                    }
                    else
                    {
                     
                    }
                }
            }

            return req.CreateResponse(HttpStatusCode.OK, "Processing completed");
       }
        

        private async Task<IEnumerable<Loan>> GetLoansAsync()
        {
            var loans = await loanOfferRepository.GetAllAsync(searchParams, userId);
            return loans;
        }

        private async Task<bool> DebitBorrowerAccountAsync(Guid borrowerId, decimal amount)
        {
            // Implement logic to debit the borrower's account
            // Example: Call a payment gateway API to perform the debit
        }

        private async Task MarkLoanAsPaidAsync(Guid loanId)
        {
            // Implement logic to update the loan status to paid in the database
        }
    }
        
    
}
