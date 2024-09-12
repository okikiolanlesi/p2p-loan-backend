using P2PLoan.Models;
using P2PLoan.Services;
using Azure.AI.OpenAI;
using Azure;
using OpenAI.Chat;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using P2PLoan.Interfaces.Services;
using P2PLoan.DTOs;


namespace P2PLoan.Services
{
    public class AIService : IAIService
    {
        private readonly IConfiguration _configuration;
        private readonly string _endpoint;
        private readonly string _apiKey;
        private readonly AzureOpenAIClient _azureClient;
        private readonly ChatClient _chatClient;

        public AIService(IConfiguration configuration)
        {
            _configuration = configuration;
            _endpoint = _configuration["AzureConfig:OpenAI:ApiUrl"];
            _apiKey = _configuration["AzureConfig:OpenAI:ApiKey"];
            _azureClient = new AzureOpenAIClient(new Uri(_endpoint), new AzureKeyCredential(_apiKey));
            _chatClient = _azureClient.GetChatClient(_configuration["AzureConfig:OpenAI:ChatEngine"]);
        }

        public async IAsyncEnumerable<string> ChatWithAI(ChatRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_endpoint) || string.IsNullOrEmpty(_apiKey))
            {
                throw new InvalidOperationException("Endpoint or API key is not configured properly.");
            }

            var history = request.ChatHistory;

            if (!history.Any(msg => msg.Role == "system"))
            {
                var systemMessage = new DTOs.ChatMessage
                {
                    Role = "system",
                    Content = $"You are an helpful AI assistant that helps people with peer to peer loan information. You SHOULD NOT give out users wallet information or information that falls under private information." + "" +
                    $" For anything other than peer to peer loan questions, respond with 'I am a helpful peer to peer loan assistant, I can only answer questions about peer to peer loans.' "
                };

                history.Insert(0, systemMessage);
            }

            history.Add(new DTOs.ChatMessage { Role = "user", Content = request.NewMessage });

            var openAiHistory = history.Select(msg => MapToOpenAIChatMessage(msg)).ToList();


            await foreach (var stream in _chatClient.CompleteChatStreamingAsync(openAiHistory, new ChatCompletionOptions() { Temperature = 0.7f, MaxTokens = 500 }, cancellationToken))
            {
                foreach (ChatMessageContentPart chunk in stream.ContentUpdate)
                {
                    yield return chunk.Text;
                }
            }
        }

        private OpenAI.Chat.ChatMessage MapToOpenAIChatMessage(DTOs.ChatMessage msg)
        {
            return msg.Role switch
            {
                "system" => new SystemChatMessage(msg.Content),
                "user" => new UserChatMessage(msg.Content),
                "assistant" => new AssistantChatMessage(msg.Content),
                _ => throw new InvalidOperationException($"Unknown role: {msg.Role}")
            };
        }
    }
}
