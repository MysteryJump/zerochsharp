using Nest;
using System;
using System.Threading.Tasks;

namespace ZerochSharp.Models.FullTextSearchService
{
    public class FullTextSearch
    {
        private static ElasticClient elasticClient;
        private const string KUROMOJI_ANALYZER_NAME = "kuromoji";

        public static FullTextSearch Instance { get; }
        public FullTextSearch(string path = "localhost")
        {
            var uri = new Uri($"http://{path}:9200/");
            var config = new ConnectionSettings(uri);
            elasticClient = new ElasticClient(config);
        }
        public static async Task InitializeElasticsearchService()
        {
            await CreateResponseIndex();
            await CreateThreadIndex();
        }
        private static async Task CreateThreadIndex()
        {
            var response = await elasticClient.Indices.CreateAsync("thread", i => i
                .Settings(s => s
                    .Analysis(a => a
                        .Analyzers(aa => aa
                            .Kuromoji(KUROMOJI_ANALYZER_NAME, k => k
                                .Mode(KuromojiTokenizationMode.Search)
                            )
                        )
                    )
                )
                .Map<FullTextSearchThreadMapping>(m => m
                    .Properties(p => p
                        .Text(t => t
                            .Name(e => e.Title)
                            .Analyzer(KUROMOJI_ANALYZER_NAME)
                        )
                        .Text(t => t
                            .Name(e => e.BoardKey)
                        )
                        .Text(t => t
                            .Name(e => e.Author)
                        )
                        .Number(n => n
                            .Name(e => e.ResponseCount)
                            .Type(NumberType.Integer)
                        )
                        .Number(n => n
                            .Name(e => e.Id)
                            .Type(NumberType.Integer)
                        )
                        .Date(d => d
                            .Name(e => e.Modified)
                        )
                        .Date(d => d
                            .Name(e => e.Created)
                        )
                    )
                )
            );
        }
        private static async Task CreateResponseIndex()
        {
            var response = await elasticClient.Indices.CreateAsync("response", i => i
                .Settings(s => s
                    .Analysis(a => a
                        .Analyzers(aa => aa
                            .Kuromoji(KUROMOJI_ANALYZER_NAME, k => k
                                .Mode(KuromojiTokenizationMode.Search)
                            )
                        )
                    )
                )
                .Map<FullTextSearchResponseMapping>(m => m
                    .Properties(p => p
                        .Text(t => t
                            .Name(e => e.Name)
                            .Analyzer(KUROMOJI_ANALYZER_NAME)
                        )
                        .Text(t => t
                            .Name(e => e.Mail)
                            .Analyzer(KUROMOJI_ANALYZER_NAME)
                        )
                        .Text(t => t
                            .Name(e => e.HostAddress)
                        )
                        .Text(t => t
                            .Name(e => e.Body)
                            .Analyzer(KUROMOJI_ANALYZER_NAME)
                        )
                        .Text(t => t
                            .Name(e => e.BoardKey)
                        )
                        .Number(n => n
                            .Name(e => e.ThreadId)
                            .Type(NumberType.Long)
                        )
                        .Number(n => n
                            .Name(e => e.Id)
                            .Type(NumberType.Long)
                        )
                        .Date(n => n
                            .Name(e => e.Created)
                        )
                    )
                )
            );
        }
        public object Search(string query)
        {

            throw new NotImplementedException();
        }
        public class SearchQuery
        {
            public string QueryString { get; set; }
            public SearchQueryType QueryType { get; set; }
        }
        [Flags]
        public enum SearchQueryType
        {
            Response = 1 << 0,
            Thread = 1 << 1
        }

        public class FullTextSearchResponseMapping : ZerochSharp.Models.Boards.IResponse
        {
            public int Id { get; set; }
            public string Body { get; set; }
            public string Mail { get; set; }
            public string Name { get; set; }
            public long ThreadId { get; set; }
            public string HostAddress { get; set; }
            public DateTime Created { get; set; }
            public string BoardKey { get; set; }
        }
        public class FullTextSearchThreadMapping
        {
            public string Title { get; set; }
            public string Author { get; set; }
            public DateTime Created { get; set; }
            public DateTime Modified { get; set; }
            public int ResponseCount { get; set; }
            public int Id { get; set; }
            public string BoardKey { get; set; }
        }

    }
}
