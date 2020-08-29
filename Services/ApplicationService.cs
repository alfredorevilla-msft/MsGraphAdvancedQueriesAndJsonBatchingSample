using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using MsGraphAdvancedQueriesAndJsonBatchingSample.Models;

namespace Services
{
    public class ApplicationService
    {
        private readonly GraphServiceClient client;

        public ApplicationService(
            DefaultAzureCredential defaultAzureCredential,
            ILogger<ApplicationService> logger
            )
        {
            client = new GraphServiceClient(new DelegateAuthenticationProvider(async message =>
            {
                var at = await defaultAzureCredential.GetTokenAsync(new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" }));
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", at.Token);

                await Task.CompletedTask;
            }));
            this.logger = logger;
        }

        private readonly ILogger<ApplicationService> logger;

        public async Task<ApplicationResponseModel> SearchAsync(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                throw new ArgumentException(nameof(searchText));
            }

            var applicationPage = await client.Applications
                .Request(new Option[] {
                new HeaderOption("consistencyLevel", "eventual"),
                new QueryOption("$count", "true"),
                new QueryOption("$search", $"\"displayName:{searchText}\"") })
                .Top(5)
                .GetAsync();

            logger.LogDebug("Building batch request");

            var batchRequestContent = new BatchRequestContent();

            var servicePrincipalsRequestId = batchRequestContent.AddBatchRequestStep(
            client.ServicePrincipals.Request()
            .Select("id,appId")
            .Filter(string.Join(" or ", applicationPage.Select(a => $"appId eq '{a.AppId}'"))));

            var ownerRequestIds = applicationPage
                .Select(a => new
                {
                    a.AppId,
                    Id = batchRequestContent.AddBatchRequestStep(client.Applications[a.Id].Owners.Request().Select("id,displayName,userPrincipalName"))
                })
                .ToDictionary(a => a.AppId, a => a.Id);

            logger.LogInformation("Sending Graph batch request");

            var returnedResponse = await client.Batch.Request().PostAsync(batchRequestContent);

            var servicePrincipalsPage = await returnedResponse.GetResponseByIdAsync<GraphServiceServicePrincipalsCollectionResponse>(servicePrincipalsRequestId);

            logger.LogDebug("Building response model");

            List<ApplicationModel> list = new List<ApplicationModel>();
            foreach (var item in applicationPage)
            {
                logger.LogDebug("Selecting owners for app id {0}", item.AppId);

                var owners = (await returnedResponse.GetResponseByIdAsync<ApplicationOwnersCollectionWithReferencesResponse>(ownerRequestIds[item.AppId])).Value;

                var servicePrincipals = servicePrincipalsPage.Value
                    .Where(p => p.AppId == item.AppId);

                logger.LogDebug("Selecting service principals for app id {0}", item.AppId);

                list.Add(new ApplicationModel
                {
                    AppId = item.AppId,
                    DisplayName = item.DisplayName,
                    ServicePrincipalsIds = servicePrincipals.Select(p => p.Id).ToArray(),
                    OwnersNames = owners.OfType<User>().Select(d => d.DisplayName).ToArray()
                });
            }

            logger.LogInformation("Returning application response model");

            return new ApplicationResponseModel(list, (long)applicationPage.AdditionalData["@odata.count"]);
        }
    }
}