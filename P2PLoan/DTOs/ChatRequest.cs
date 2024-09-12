using System.Collections.Generic;

namespace P2PLoan.DTOs
{
    public class ChatRequest
    {
        public List<ChatMessage> ChatHistory { get; set; } = [];
        public required string NewMessage { get; set; }

    }
}

