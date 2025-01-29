using iText.Forms;
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

        /// <summary>
        /// Error while signing multiple signature fields using itext c#
        /// https://stackoverflow.com/questions/79396411/error-while-signing-multiple-signature-fields-using-itext-c-sharp
        /// 
        /// Indeed, using the OP's code Acrobat has issues when applying multiple signatures in one go.
        /// But see <see cref="CreateLikeDharmendraSinsinwarImproved"/>
        /// </summary>
        [Test]
        public void CreateLikeDharmendraSinsinwar()
        {
            string DEST = @"..\..\..\target\test-outputs\signature\FileWith3SignatureFields.pdf";
            PdfWriter writer = new PdfWriter(DEST);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document document = new Document(pdfDoc);

            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);

            AddSignatureFieldDharmendraSinsinwar(form, "signature1", 36, 700, 200, 50);
            AddSignatureFieldDharmendraSinsinwar(form, "signature2", 36, 600, 200, 50);
            AddSignatureFieldDharmendraSinsinwar(form, "signature3", 36, 500, 200, 50);

            document.Close();
        }

        /// <see cref="CreateLikeDharmendraSinsinwar"/>
        public static void AddSignatureFieldDharmendraSinsinwar(PdfAcroForm form, string fieldName, float x, float y, float width, float height)
        {
            Rectangle rect = new Rectangle(x, y, width, height);
            PdfWidgetAnnotation pdfWidgetAnnotation = new PdfWidgetAnnotation(rect);
            PdfSignatureFormField signatureField = PdfFormCreator.CreateSignatureFormField(pdfWidgetAnnotation, form.GetPdfDocument());
            signatureField.SetFieldName(fieldName);
            form.AddField(signatureField);
        }

        /// <summary>
        /// Error while signing multiple signature fields using itext c#
        /// https://stackoverflow.com/questions/79396411/error-while-signing-multiple-signature-fields-using-itext-c-sharp
        /// 
        /// As it turned out, Acrobat requires the widget Type entry. So to make <see cref="CreateLikeDharmendraSinsinwar"/>
        /// create PDFs Acrobat can handle well, it suffices to change <see cref="AddSignatureFieldDharmendraSinsinwar(PdfAcroForm, string, float, float, float, float)"/>
        /// like here in <see cref="AddSignatureFieldDharmendraSinsinwarImproved(PdfAcroForm, string, float, float, float, float)"/>.
        /// </summary>
        [Test]
        public void CreateLikeDharmendraSinsinwarImproved()
        {
            string DEST = @"..\..\..\target\test-outputs\signature\FileWith3SignatureFieldsImproved.pdf";
            PdfWriter writer = new PdfWriter(DEST);
            PdfDocument pdfDoc = new PdfDocument(writer);
            Document document = new Document(pdfDoc);

            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, true);

            AddSignatureFieldDharmendraSinsinwarImproved(form, "signature1", 36, 700, 200, 50);
            AddSignatureFieldDharmendraSinsinwarImproved(form, "signature2", 36, 600, 200, 50);
            AddSignatureFieldDharmendraSinsinwarImproved(form, "signature3", 36, 500, 200, 50);

            document.Close();
        }

        /// <see cref="CreateLikeDharmendraSinsinwarImproved"/>
        public static void AddSignatureFieldDharmendraSinsinwarImproved(PdfAcroForm form, string fieldName, float x, float y, float width, float height)
        {
            Rectangle rect = new Rectangle(x, y, width, height);
            PdfWidgetAnnotation pdfWidgetAnnotation = new PdfWidgetAnnotation(rect);
            pdfWidgetAnnotation.Put(PdfName.Type, PdfName.Annot);
            PdfSignatureFormField signatureField = PdfFormCreator.CreateSignatureFormField(pdfWidgetAnnotation, form.GetPdfDocument());
            signatureField.SetFieldName(fieldName);
            form.AddField(signatureField);
        }
    }
}
