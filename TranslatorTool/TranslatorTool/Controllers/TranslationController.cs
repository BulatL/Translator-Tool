using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TranslatorTool.Services.Interfaces;

namespace TranslatorTool.Controllers
{
   public class TranslationController : Controller
   {
      private IXmlService _xmlService;

      public TranslationController(IXmlService xmlService)
      {
         _xmlService = xmlService;
      }
      [Route("/api/translation/{text}")]
      public async Task<IActionResult> Translation(string text)
      {
         if (String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text))
            return BadRequest();

         string translatedText = await _xmlService.Translate(text);
         return Json(translatedText);
      }

      public IActionResult CreateXmlFile()
      {
         _xmlService.CreateFile();
         return Ok();
      }
   }
}