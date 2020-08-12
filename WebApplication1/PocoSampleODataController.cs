
namespace WebApplication1
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNet.OData;
    using Microsoft.AspNet.OData.Query;
    using Microsoft.AspNet.OData.Routing;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Cosmos;
    using Microsoft.EntityFrameworkCore;

    [Route("odata/poco")]
    [ODataRoutePrefix("poco")]
    [ApiController]
    public class PocoSampleODataController : ODataController
    {
        private readonly AccountsContext accountsContext;
        private readonly CosmosClient repository;
        private const string resourceType = "Microsoft.Maps/accounts";

        public PocoSampleODataController(AccountsContext accountsContext, CosmosClient repository)
        {
            this.accountsContext = accountsContext;
            this.repository = repository;
        }

        [HttpGet("")]
        [EnableQuery(
            AllowedQueryOptions =
            AllowedQueryOptions.Filter |
            AllowedQueryOptions.OrderBy |
            AllowedQueryOptions.Top |
            AllowedQueryOptions.Skip,
            PageSize = 32)]
        [ApiVersion("v1")]
        public IQueryable<PocoSample> QueryAccounts()
        {
            accountsContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return this.accountsContext.Accounts.Where(a => a.ResourceType == resourceType);
        }

        [HttpGet("{partitionKey:guid}")]
        [EnableQuery(PageSize = 32)]
        [ApiVersion("v1")]
        public async Task<IQueryable<PocoSample>> QueryAccounts([FromRoute] Guid partitionKey)
        {
            var accountContainer = repository.GetContainer(Startup.DatabaseName, Startup.ContainerName);
            var queryString = $"SELECT * FROM c WHERE c.pk=@partitionKey and c.resourceType='{resourceType}'";
            QueryDefinition queryDefinition = new QueryDefinition(queryString)
                .WithParameter("@partitionKey", partitionKey);

            FeedIterator<PocoSample> feedIterator = accountContainer.GetItemQueryIterator<PocoSample>(
                queryDefinition,
                null,
                new QueryRequestOptions()
                {
                    MaxConcurrency = -1,
                    PartitionKey = new PartitionKey(partitionKey.ToString()),
                    MaxItemCount = 800
                });

            FeedResponse<PocoSample> queryResult;
            try
            {
                queryResult = await feedIterator.ReadNextAsync(HttpContext.RequestAborted);
            }
            catch (CosmosException documentClientException) when (documentClientException.StatusCode == HttpStatusCode.BadRequest
                || documentClientException.StatusCode == HttpStatusCode.NotFound)
            {
                return Enumerable.Empty<PocoSample>().AsQueryable();
            }
            catch (CosmosException otherException)
            {
                throw new ErrorResponseExceptionBuilder()
                    .WithStatusCode(otherException.StatusCode)
                    .WithMessage(otherException.Message)
                    .Result();
            }

            return queryResult?.AsQueryable();
        }
    }
}
