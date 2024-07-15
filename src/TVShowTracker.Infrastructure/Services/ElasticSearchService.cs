using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;

namespace TVShowTracker.Infrastructure.Services;

public class ElasticSearchService : ISearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticSearchService> _logger;
    private const string ShowIndexName = "shows";

    public ElasticSearchService(IConfiguration configuration, ILogger<ElasticSearchService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var url = configuration["ElasticSearch:Url"];
        var defaultIndex = configuration["ElasticSearch:DefaultIndex"];

        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(defaultIndex))
        {
            throw new ArgumentException("Elasticsearch configuration is missing or invalid.");
        }

        var settings = new ElasticsearchClientSettings(new Uri(url)).DefaultIndex(defaultIndex);

        _client = new ElasticsearchClient(settings);
    }

    public async Task<IEnumerable<Show>> SearchShowsAsync(string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                _logger.LogWarning("Attempted to search with null or empty query");
                return Enumerable.Empty<Show>();
            }

            var searchResponse = await _client.SearchAsync<Show>(s => s
                .Index(ShowIndexName)
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(new[] { "name^3", "overview" })
                        .Query(query)
                        .Type(TextQueryType.BestFields)
                        .Fuzziness(new Fuzziness("AUTO"))
                    )
                )
            );

            if (!searchResponse.IsValidResponse)
            {
                _logger.LogError($"Elasticsearch query failed: {searchResponse.ElasticsearchServerError?.Error?.Reason}");
                return Enumerable.Empty<Show>();
            }

            return searchResponse.Documents ?? Enumerable.Empty<Show>();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while searching for shows");
            return Enumerable.Empty<Show>();
        }
        
    }

    public async Task IndexShowAsync(Show show)
    {
        try
        {
            if (show == null)
            {
                throw new ArgumentNullException(nameof(show));
            }

            var response = await _client.IndexAsync(show, i => i.Index(ShowIndexName));

            if (!response.IsValidResponse)
            {
                _logger.LogError($"Failed to index show: {response.ElasticsearchServerError?.Error?.Reason}");
                throw new Exception("Failed to index show");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while indexing show");
            throw;
        }
    }

    public async Task IndexShowsAsync(IEnumerable<Show> shows)
    {
        if (shows == null || !shows.Any())
        {
            throw new ArgumentException("Shows collection is null or empty", nameof(shows));
        }

        try
        {
            var bulkResponse = await _client.BulkAsync(b => b
                .Index(ShowIndexName)
                .IndexMany(shows));

            if (!bulkResponse.IsValidResponse)
            {
                _logger.LogError("Failed to index shows: {ErrorReason}", bulkResponse.ElasticsearchServerError?.Error?.Reason);
                throw new Exception("Failed to index shows");
            }

            if (bulkResponse.Errors)
            {
                var failedDocuments = bulkResponse.Items.Where(i => i.IsValid == false);
                _logger.LogError("Some documents failed to index. Count: {FailureCount}", failedDocuments.Count());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while indexing shows");
            throw;
        }
        
    }
}