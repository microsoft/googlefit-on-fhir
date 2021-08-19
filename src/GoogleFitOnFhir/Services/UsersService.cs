using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using GoogleFitOnFhir.Clients.GoogleFit.Responses;
using GoogleFitOnFhir.Models;
using GoogleFitOnFhir.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using GoogleFitClient = GoogleFitOnFhir.Clients.GoogleFit.Client;

namespace GoogleFitOnFhir.Services
{
    /// <summary>
    /// User Service.
    /// </summary>
    public class UsersService : IUsersService
    {
        private readonly IUsersTableRepository usersTableRepository;

        private readonly GoogleFitClient googleFitClient;

        private readonly EventHubProducerClient eventHubProducerClient;

        private readonly ILogger<UsersService> logger;

        private readonly IUsersKeyvaultRepository usersKeyvaultRepository;

        public UsersService(
            IUsersTableRepository usersTableRepository,
            GoogleFitClient googleFitClient,
            EventHubProducerClient eventHubProducerClient,
            IUsersKeyvaultRepository usersKeyvaultRepository,
            ILogger<UsersService> logger)
        {
            this.usersTableRepository = usersTableRepository;
            this.googleFitClient = googleFitClient;
            this.eventHubProducerClient = eventHubProducerClient;
            this.usersKeyvaultRepository = usersKeyvaultRepository;
            this.logger = logger;
        }

        public async Task<User> Initiate(string authCode)
        {
            var tokenResponse = await this.googleFitClient.AuthTokensRequest(authCode);
            if (tokenResponse == null)
            {
                throw new Exception("Token response empty");
            }

            var emailResponse = await this.googleFitClient.MyEmailRequest(tokenResponse.AccessToken);
            if (emailResponse == null)
            {
                throw new Exception("Email response empty");
            }

            // Hash email to reduce characterset for KV secret name compatability
            var userId = Utility.Base58String(emailResponse.EmailAddress);
            var user = new User(userId);

            // Insert user into UsersTable
            this.usersTableRepository.Upsert(user);

            // Insert refresh token into users KV by userId
            this.usersKeyvaultRepository.Upsert(userId, tokenResponse.RefreshToken);

            return user;
        }

        public async void ImportFitnessData(User user)
        {
            string refreshToken;

            try
            {
                refreshToken = await this.usersKeyvaultRepository.GetByName(user.Id);
            }
            catch (AggregateException ex)
            {
                this.logger.LogError(ex.Message);
                return;
            }

            AuthTokensResponse tokensResponse = await this.googleFitClient.RefreshTokensRequest(refreshToken);

            // TODO: Store new refresh token
            var dataSourcesList = await this.googleFitClient.DatasourcesListRequest(tokensResponse.AccessToken);

            // Create a batch of events for IoMT eventhub
            using EventDataBatch eventBatch = await this.eventHubProducerClient.CreateBatchAsync();

            // Get user's info for LastSync date
            var userInfo = this.usersTableRepository.GetById(user.Id);

            // Generating datasetId based on event type
            var startDate = DateTime.UnixEpoch;
            if (userInfo.LastSync != null)
            {
                // previous hour for interval migration
                Console.WriteLine("hour");
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour - 1, DateTime.Now.Minute, DateTime.Now.Second);
            }
            else
            {
                // last 30 days to beginning of hour for first migration
                Console.WriteLine("initial");
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                // Console.WriteLine(new DateTimeOffset(nowMinusMonth).ToUnixTimeMilliseconds() * 1000000);
                // Console.WriteLine(DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1000000);
            }

            Console.WriteLine(startDate);

            // Get dataset for each dataSource
            foreach (var datasourceId in dataSourcesList.DatasourceIds)
            {
                var dataset = await this.googleFitClient.DatasetRequest(
                    tokensResponse.AccessToken,
                    datasourceId,
                    "1574159699023000000-1574159699023000000");

                // Add user id to payload
                // TODO: Use userId from queue message
                dataset.UserId = "testUserId";

                // Push dataset to IoMT connector
                if (!eventBatch.TryAdd(new EventData(JsonConvert.SerializeObject(dataset))))
                {
                    throw new Exception("Event is too large for the batch and cannot be sent.");
                }
            }

            try
            {
                // Use the producer client to send the batch of events to the event hub
                await this.eventHubProducerClient.SendAsync(eventBatch);
                this.logger.LogInformation("A batch of events has been published.");

                // Update LastSync column
                user.LastSync = DateTime.Now;
                this.usersTableRepository.Update(user);
            }
            finally
            {
                await this.eventHubProducerClient.DisposeAsync();
            }
        }

        public void QueueFitnessImport(User user)
        {
        }
    }
}
