using System.Threading.Tasks;
using TranslatorTool.Models;

namespace TranslatorTool.Services.Interfaces
{
   public interface IXmlService
   {

      void CreateFile();
      string FindElementByFrom(string text);
      void Write(string fromText, string toText);
      Task<string> Translate(string text);
   }
}