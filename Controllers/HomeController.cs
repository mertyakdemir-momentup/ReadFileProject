using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ReadFileProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var byteLength = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);

                        page.Header().Column(column =>
                        {
                            column.Item().AlignCenter().Row(row =>
                            {
                                row.ConstantItem(124).Column(subColumn =>
                                {
                                    var logo = GetStreamFromUrl();
                                    subColumn.Item().Image(logo);
                                })
                                ;
                            });
                        });
                    });
                })
               .GeneratePdf();

                return Ok(byteLength.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            };
        }

        private Stream GetStreamFromUrl()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var responseMessage = client.GetAsync("http://stage.memotive.io/api/v1/requests/image/clients/6uUyq5995lbI.png").Result;
                var s3ResponseMessage = client.GetAsync("https://s3.cloud.ngn.com.tr/memotive/documents/0/0/0/0/0/6a1a9fb7-5060-414a-99fd-52f7a5335878.jpg").Result;

                return responseMessage.Content.ReadAsStreamAsync().Result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
