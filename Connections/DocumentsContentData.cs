using CPTest.Data;
using ClinicalXPDataConnections.Models;
using ClinicalXPDataConnections.Data;

namespace CPTest.Connections
{
    interface IDocumentsContentData
    {
        public DocumentsContent GetDocumentContent(int docID);
    }
    public class DocumentsContentData : IDocumentsContentData
    {
        private readonly DocumentContext _context;
        public DocumentsContentData(DocumentContext context)
        {
            _context = context;
        }
        
        public DocumentsContent GetDocumentContent(int docID)
        {
            DocumentsContent doc = _context.DocumentsContent.FirstOrDefault(c => c.DocContentID == docID);
            //We must use "contains" because half of the constant codes have trailing spaces for some reason!
            return doc;
        }
        
    }
}
