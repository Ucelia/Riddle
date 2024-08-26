using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace RiddelMyExam.Pages
{
//    public class GivePdf
//    {
//        public byte[] GeneratePdf(List<Questionsss> questions)
//        {
//            var pdfStream = new MemoryStream();

//            using (var document = new Document())
//            {
//                using (var writer = PdfWriter.GetInstance(document, pdfStream))
//                {
//                    document.Open();

//                    foreach (var question in questions)
//                    {
//                        if (question.QuestionText != null)
//                        {
//                            PdfPTable table = new PdfPTable(1);
//                            table.AddCell(new PdfPCell(new Phrase(question.QuestionText)));

//                            // Add other question details here using additional cells in the table

//                            document.Add(table);
//                        }
//                    }

//                    // Close the document after adding content
//                    document.Close();
//                }
//            }

//            // Reset the position of the MemoryStream
//            pdfStream.Position = 0;

//            return pdfStream.ToArray();
//        }


//    }
//}
}
    