using CPTest.Data;
using CPTest.Models;

namespace CPTest.Connections
{
    interface IDocumentsContentData
    {
        public DocumentsContent GetDocumentContent(int docID);
    }
    public class DocumentsContentData : IDocumentsContentData
    {
        private readonly DataContext _context;
        public DocumentsContentData(DataContext context)
        {
            _context = context;
        }
        
        public DocumentsContent GetDocumentContent(int docID)
        {
            var doc = _context.DocumentsContent.FirstOrDefault(c => c.DocContentID == docID);
            //We must use "contains" because half of the constant codes have trailing spaces for some reason!
            return doc;
        }
        
    }
}
