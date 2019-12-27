using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TranslatorTool.Services.Interfaces;
using GoogleTranslateFreeApi;
using System.Threading.Tasks;

namespace TranslatorTool.Services
{
   public class XmlService : IXmlService
   {
      private readonly IHostingEnvironment _hostingEnvironment;

      public XmlService(IHostingEnvironment hostingEnvironment)
      {
         _hostingEnvironment = hostingEnvironment;
      }

      public async Task<string> Translate(string textForTranslate)
      {
         string translatedText = FindElementByFrom(textForTranslate);
         if (String.IsNullOrEmpty(translatedText))
         {
            GoogleTranslator googleTranslator = new GoogleTranslator();
            TranslationResult result = await googleTranslator.TranslateAsync(textForTranslate, Language.Auto, Language.English);

            translatedText = result.MergedTranslation;

            Write(textForTranslate, translatedText);
         }
         return translatedText;
      }

      public string FindElementByFrom(string text)
      {
         string path = Path.Combine(_hostingEnvironment.WebRootPath, "Translations.xml");
         try
         {
            XDocument xDoc = XDocument.Load(path);

            string result = xDoc.Descendants("translation")
                .FirstOrDefault(p => p.Elements("from")
                  .Any(c => c.Value.Equals(text))
                )?.Element("to").Value;
            return result;
         }
         catch (FileNotFoundException)
         {
            throw new FileNotFoundException();
         }
      }

      public void Write(string textForTranslate, string translatedText)
      {
         string path = Path.Combine(_hostingEnvironment.WebRootPath, "Translations.xml");

         XElement newTranslation = new XElement("translation",
                           new XElement("from", textForTranslate),
                           new XElement("to", translatedText));

         newTranslation.SetAttributeValue("timestamp", DateTime.Now.ToString());

         if (!File.Exists(path))
         {
            newTranslation.SetAttributeValue("id", 1);
            new XDocument(new XElement("translations", newTranslation)).Save(path);
         }
         else
         {
            XDocument xDoc = XDocument.Load(path);
            int count = xDoc.Descendants("translation").Count();
            newTranslation.SetAttributeValue("id", count + 1);
            xDoc.Root.Add(newTranslation);
            xDoc.Save(path);
         }
      }

      public void CreateFile()
      {
         string path = Path.Combine(_hostingEnvironment.WebRootPath, "Translations.xml");
         new XDocument(
             new XElement("translations")
         )
         .Save(path);
      }
   }
}
