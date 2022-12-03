using AldasKevin_Exm.layer;
using EXMAPI_TEXTDDESCRIPTION_ALDAS.Desc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace EXMAPI_TEXTDDESCRIPTION_ALDAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImgController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<string>> PostImagen(string ruta)
        {
            // Get of Data by API
            var api = new System.Net.WebClient();
            api.Headers.Add("Content_Type", "application/octec-stream");
            api.Headers.Add("Content_Type", "application/json");
            api.Headers.Add("Ocp-Apim-Subscription-Key", "d2b6384bd9f1453ba1b819a6b8737911");

            // Get of description by IMG
            var qs_descrip = "maxCandidates=1&language=es&model-version=latest";
            var url_descrip = "https://eastus.api.cognitive.microsoft.com/vision/v3.2/describe";
            var resp_descrip = api.UploadFile(url_descrip + "?" + qs_descrip, "POST", ruta);
            var json_descrip = Encoding.UTF8.GetString(resp_descrip);
            var _descrip = Newtonsoft.Json.JsonConvert.DeserializeObject<respuesta_desc>(json_descrip);

            // Get of Text by IMG
            var qs_text = "language=es&language=true&model-version=latest";
            var url_text = "https://eastus.api.cognitive.microsoft.com/vision/v3.2/ocr";
            var resp_text = api.UploadFile(url_text + "?" + qs_text, "POST", ruta);
            var json_text = Encoding.UTF8.GetString(resp_text);
            var text = Newtonsoft.Json.JsonConvert.DeserializeObject<OCR_Respuesta>(json_text);
           
            return Ok(formatText(text) + "\n" + formatDescrip(_descrip));
        }


        private static string formatDescrip(respuesta_desc seri)
        {
            var respuesta = "<h2>Description of picture:</h2><br/>";
            foreach (var x in seri.description.captions)
            {
                respuesta +=  x.text + "<br/>";
            }
            return respuesta;

        }

        private static string formatText(OCR_Respuesta resp)
        {
            var respuesta = "<h2>TEXT:</h2><br/>";
            foreach (var region in resp.regions)
            {
                respuesta += "<p>";
                foreach (var line in region.lines)
                {
                    foreach (var word in line.words)
                    {
                        respuesta += word.text + " ";
                    }
                    respuesta += "<br/>";
                }
                respuesta += "</p>";
            }
            return respuesta;

        }

       
    }
}
