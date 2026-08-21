using ASPHW3_Services__DI_.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace ASPHW3_Services__DI_.Formatters
{
    public class BookVCardOutputFormatter : TextOutputFormatter
    {
        public BookVCardOutputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/vcard"));
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type? type)
        {
            if (typeof(IEnumerable<Book>).IsAssignableFrom(type))
                return true;
            if (typeof(Book).IsAssignableFrom(type))
                return true;
            return false;
        }
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;

            if (context.Object is IEnumerable<Book> books)
            {
                foreach (var book in books)
                {
                    await WriteVCard(book, response);
                }
            }
            else
            {
                await WriteVCard((Book)context.Object!, response);
            }
        }

        private static Task WriteVCard(Book book, HttpResponse response)
        {
            return response.WriteAsync($@"
                BEGIN:VCARD
                VERSION:3.0
                FN:{book.Title}
                NOTE:Author = {book.Author}
                NOTE:Category = {book.Category}
                NOTE:Price = {book.Price}
                NOTE:Stock = {book.Stock}
                NOTE:PageCount = {book.PageCount}
                NOTE:Year = {book.PublishedYear}
                NOTE:IsAvailable = {book.IsAvailable}
                END:VCARD
");
        }
    }
}
