using ASPHW3_Services__DI_.DTOs;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;

namespace ASPHW3_Services__DI_.Formatters
{
    public class BookVCardInputFormatter : TextInputFormatter
    {
        public BookVCardInputFormatter()
        {
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/vcard"));

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanReadType(Type type)
        {
            if (typeof(BookAddDto).IsAssignableFrom(type))
                return true;
            return false;
        }
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            var httpContext = context.HttpContext;
            var serviceProvider = httpContext.RequestServices;

            using var reader = new StreamReader(httpContext.Request.Body);
            string? nameLine = null;

            try
            {
                await ReadLineAsync("BEGIN:VCARD", reader, context);
                await ReadLineAsync("VERSION", reader, context);

                nameLine = await ReadLineAsync("FN:", reader, context);
                var titleLine = await ReadLineAsync("NOTE:Title = ", reader, context);
                var authorLine = await ReadLineAsync("NOTE:Author = ", reader, context);
                var categoryLine = await ReadLineAsync("NOTE:Category = ", reader, context);
                var priceLine = await ReadLineAsync("NOTE:Price = ", reader, context);
                var stockLine = await ReadLineAsync("NOTE:Stock = ", reader, context);
                var pageCountLine = await ReadLineAsync("NOTE:PageCount = ", reader, context);
                var yearLine = await ReadLineAsync("NOTE:Year = ", reader, context);

                await ReadLineAsync("END:VCARD", reader, context);

                var dto = new BookAddDto
                {
                    Title = titleLine,
                    Author = authorLine,
                    Category = categoryLine,
                    Price = decimal.Parse(priceLine),
                    Stock = int.Parse(stockLine),
                    PageCount = int.Parse(pageCountLine),
                    PublishedYear = int.Parse(yearLine)
                };

                return await InputFormatterResult.SuccessAsync(dto);

            }
            catch (Exception ex)
            {

                return await InputFormatterResult.FailureAsync();
            }
        }

        private static async Task<string> ReadLineAsync(string expectedText, StreamReader reader,
         InputFormatterContext context)
        {
            var line = await reader.ReadLineAsync();

            if (line is null || !line.StartsWith(expectedText))
            {
                var errorMessage = $"Looked for '{expectedText}' and got '{line}'";

                context.ModelState.TryAddModelError(context.ModelName,errorMessage);

                throw new Exception(errorMessage);
            }

            return line.Substring(expectedText.Length).Trim();
        }

    }
}
