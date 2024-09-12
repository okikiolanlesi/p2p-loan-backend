using P2PLoan.DTOs;
using P2PLoan.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace P2PLoan.Interfaces.Services
{
    public interface IAIService
    {
        IAsyncEnumerable<string> ChatWithAI(ChatRequest request, CancellationToken cancellationToken);
    }
}