using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;

        public TaskController(ILogger<TaskController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public string Get()
        {
            var result = HttpContext.Session.GetString("Result");
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            Request.Headers.TryGetValue("page-size", out var test);
            var bodyStr = "";

            using (StreamReader reader
                      = new StreamReader(Request.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyStr = await reader.ReadToEndAsync();
            }
            int allowedChars = int.Parse(test);
            var result = GetLines(bodyStr, allowedChars);
            HttpContext.Session.SetString("Result", result);

            return StatusCode(201, bodyStr);
        }


        private string GetLines(string words, int allowedChars)
        {
            string result = "";
            var wordlist = words.Split(' ', ',').ToList();
            int i = allowedChars;
            foreach (var word in wordlist)
            {
                if (word.Length <= i)
                {
                    result += $"{word} ";
                    i -= word.Length + 1;
                }
                else if (word.Length <= allowedChars)
                {
                    i = allowedChars;
                    if (word.Length <= i)
                    {
                        result += $"\n{word} ";
                        i -= word.Length + 1;
                    }
                }
                if (i <= 0)
                {
                    result += "\n";
                    i = allowedChars;
                }
            }

            return result;
        }
    }
}
