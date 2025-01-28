using iText.Forms.Fields;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Annot;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Signatures;
using NUnit.Framework;

namespace iText9.Net_Playground.Signature
{
    internal class AddSignatureField
    {
        /// <summary>
        /// IText7 v9 Signature Field is Not Visible After Adding it to PDF
        /// https://stackoverflow.com/questions/79385947/itext7-v9-signature-field-is-not-visible-after-adding-it-to-pdf
        /// 
        /// There indeed is no visible visualization of the signature field. But the construction also 
        /// does not ask for some appearance.
        /// </summary>
        /// <see cref="CreateLikeBuilttospill22Improved"/>
        [Test]
        public void CreateLikeBuilttospill22()
        {
            Directory.CreateDirectory(@"..\..\..\target\test-outputs\signature\");
            string fileWithoutField = @"..\..\..\target\test-outputs\signature\FileWithoutSignatureField.pdf";
            string fileWithField = @"..\..\..\target\test-outputs\signature\FileWithSignatureField.pdf";

            byte[] pdfBytes = CreateSimplePdf();
            File.WriteAllBytes(fileWithoutField, pdfBytes);

            using var outputMemoryStream = new MemoryStream();
            using var memoryStream = new MemoryStream(pdfBytes);
            using var reader = new PdfReader(memoryStream);
            using var writer = new PdfWriter(outputMemoryStream);
            using var pdfDoc = new PdfDocument(reader, writer);
            using var document = new Document(pdfDoc);

            //Creates a signature form field
            PdfSignatureFormField field = new SignatureFormFieldBuilder(pdfDoc, "SIGNNAME")
            .SetWidgetRectangle(new Rectangle(100, 150, 200, 100)).CreateSignature();

            field.GetWidgets()[0].SetHighlightMode(PdfAnnotation.HIGHLIGHT_OUTLINE).SetFlags(PdfAnnotation.PRINT);

            PdfFormCreator.GetAcroForm(pdfDoc, true).AddField(field);

            pdfDoc.Close();

            File.WriteAllBytes(fileWithField, outputMemoryStream.ToArray());
        }

        /// <summary>
        /// IText7 v9 Signature Field is Not Visible After Adding it to PDF
        /// https://stackoverflow.com/questions/79385947/itext7-v9-signature-field-is-not-visible-after-adding-it-to-pdf
        /// 
        /// To make the signature field appear even in PDF viewers that do not support digital signature fields,
        /// you have to provide an appearance, e.g. a red rectangular path like here.
        /// </summary>
        /// <see cref="CreateLikeBuilttospill22"/>
        [Test]
        public void CreateLikeBuilttospill22Improved()
        {
            Directory.CreateDirectory(@"..\..\..\target\test-outputs\signature\");
            string fileWithField = @"..\..\..\target\test-outputs\signature\FileWithSignatureFieldImproved.pdf";

            byte[] pdfBytes = CreateSimplePdf();

            using var outputMemoryStream = new MemoryStream();
            using var memoryStream = new MemoryStream(pdfBytes);
            using var reader = new PdfReader(memoryStream);
            using var writer = new PdfWriter(outputMemoryStream);
            using var pdfDoc = new PdfDocument(reader, writer);
            using var document = new Document(pdfDoc);

            //Creates a signature form field
            PdfSignatureFormField field = new SignatureFormFieldBuilder(pdfDoc, "SIGNNAME")
            .SetWidgetRectangle(new Rectangle(100, 150, 200, 100)).CreateSignature();

            PdfFormXObject appearance = new PdfFormXObject(new Rectangle(200, 100));
            PdfCanvas canvas = new PdfCanvas(appearance, pdfDoc);
            canvas.SetStrokeColorRgb(1, 0, 0);
            canvas.Rectangle(0, 0, 200, 100);
            canvas.Stroke();

            field.GetWidgets()[0].SetAppearance(PdfName.N, appearance.GetPdfObject());

            field.GetWidgets()[0].SetHighlightMode(PdfAnnotation.HIGHLIGHT_OUTLINE).SetFlags(PdfAnnotation.PRINT);

            PdfFormCreator.GetAcroForm(pdfDoc, true).AddField(field);

            pdfDoc.Close();

            File.WriteAllBytes(fileWithField, outputMemoryStream.ToArray());
        }

        /// <see cref="CreateLikeBuilttospill22"/>
        /// <see cref="CreateLikeBuilttospill22Improved"/>
        byte[] CreateSimplePdf()
        {
            using var outputMemoryStream = new MemoryStream();
            using (var writer = new PdfWriter(outputMemoryStream))
            {
                using var pdfDocument = new PdfDocument(writer);
            }
            return outputMemoryStream.ToArray();
        }
    }
}
