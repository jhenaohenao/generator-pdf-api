using Formatos.Pdf.Core.DTOs;
using Formatos.Pdf.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Formatos.Pdf.Api.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar la conversión de HTML a PDF.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly IPdfService _pdfService;

        /// <summary>
        /// Constructor que recibe las dependencias a través de inyección.
        /// </summary>
        /// <param name="pdfService">Servicio encargado de la conversión de HTML a PDF.</param>
        public PdfController(IPdfService pdfService)
        {
            _pdfService = pdfService;
        }

        /// <summary>
        /// Convierte un contenido HTML proporcionado a un archivo PDF y lo devuelve.
        /// </summary>
        /// <param name="contentDto">Objeto que contiene el contenido HTML a convertir.</param>
        /// <returns>Un archivo PDF generado a partir del contenido HTML proporcionado.</returns>
        /// 
        //[HttpPost("convert")]
        //public IActionResult ConvertHtmlToPdf([FromBody] HtmlContentDto contentDto)
        //{
        //    var pdf = _pdfService.ConvertHtmlToPdf(contentDto.Html);
        //    return File(pdf, "application/pdf", "document.pdf");
        //}
        //[HttpPost("convert")]
        //public IActionResult ConvertHtmlToPdf([FromBody] HtmlContentDto contentDto)
        //{
        //    var pdf = _pdfService.ConvertHtmlToPdf(contentDto.Html);
        //    return File(pdf, "application/pdf", "document.pdf");
        //}

        [HttpPost("convert")]
        public IActionResult ConvertHtmlToPdf([FromBody] HtmlContentDto contentDto)
        {
            // Convierte el HTML a PDF
            var pdf = _pdfService.ConvertHtmlToPdf(contentDto.Html);

            // Define la ruta donde quieres guardar el archivo PDF en el servidor
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "SavedPdfs");

            // Crea el directorio si no existe
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            // Define el nombre del archivo
            var fileName = "document.pdf";
            var fullPath = Path.Combine(pathToSave, fileName);

            // Guarda el archivo en el sistema de archivos del servidor
            System.IO.File.WriteAllBytes(fullPath, pdf);

            // Puedes retornar una respuesta de éxito con la ruta del archivo o cualquier otra información relevante
            return Ok(new { Message = "PDF generado y guardado en el servidor", Path = fullPath });
        }
        /// <summary>
        /// Proporciona un template HTML de ejemplo.
        /// </summary>
        /// <returns>Un template HTML de prueba.</returns>
        [HttpGet("get-template")]
        public IActionResult GetHtmlTemplate()
        {
            string template = "<div>Hola, este es un template de prueba.</div>";
            return Ok(template);
        }
    }
}
