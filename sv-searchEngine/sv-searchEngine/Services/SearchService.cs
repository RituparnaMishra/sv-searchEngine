using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using sv_searchEngine.Models.DTOS;

namespace sv_searchEngine.Services
{

    public class SearchService
    {
        private static string _luceneDir =
 Path.Combine(System.IO.Directory.GetCurrentDirectory(), "lucene_index");
     
        //private  FSDirectory _directory() { 

            
        //        if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
        //        if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
        //        var lockFilePath = Path.Combine(_luceneDir, "write.lock");
        //        if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
        //        return _directoryTemp;
            
        //}
            Lucene.Net.Store.FSDirectory createIndex(IEnumerable<Building> Buildings)
        {
            var directory = new SimpleFSDirectory(new DirectoryInfo(_luceneDir));

            using (StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
            using (var writer = new IndexWriter(directory, analyzer, new IndexWriter.MaxFieldLength(1000)))
            { // the writer and analyzer will popuplate the directory with documents

                foreach (Building building in Buildings)
                {
                    var document = new Document();

                    document.Add(new Field("Name", building.Name.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("Description", building.Description.ToString(), Field.Store.YES, Field.Index.ANALYZED));

                    document.Add(new Field("ID", building.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

                    writer.AddDocument(document);
                }

                writer.Optimize();
                writer.Flush(true, true, true);
            }

            return directory;
        }

        public IEnumerable<Building> search(string searchCriteria, IEnumerable<Building> Buildings)
        {
            var searchedBuildings = new List<Building>();
            var Index = createIndex(Buildings);

            using (var reader = IndexReader.Open(Index, true))
            using (var searcher = new IndexSearcher(reader))
            {
                using (StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30))
                {
                    var queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "FullText", analyzer);

                    queryParser.AllowLeadingWildcard = true;

                    var query = queryParser.Parse(searchCriteria);

                    var collector = TopScoreDocCollector.Create(1000, true);

                    searcher.Search(query, collector);

                    var matches = collector.TopDocs().ScoreDocs;

                    foreach (var item in matches)
                    {
                        var id = item.Doc;
                        var doc = searcher.Doc(id);

                        searchedBuildings.Add(
                            new Building()
                            {
                                Name = doc.GetField("Name").StringValue,
                                Description = doc.GetField("Description").StringValue,
                                Id = new Guid(doc.GetField("ID").StringValue)

                            });

                       

                    }
                }
            }

            return searchedBuildings;

        }

    }
}