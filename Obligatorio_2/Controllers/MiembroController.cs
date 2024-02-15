using Logica_De_Negocio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using static System.Net.WebRequestMethods;

namespace Obligatorio_2.Controllers
{
    public class MiembroController : Controller
    {
        private Sistema _miSistema = Sistema.Instancia;

        public IActionResult VerPost()
        {
            string email = HttpContext.Session.GetString("email");

            ViewBag.Posts = _miSistema.DevolverPostParaUsuario(email);
            
            return View();
        }

        public IActionResult EnviarSolicitud()
        {
            return View();
        }
        public IActionResult VerSolicitudes()
        {
            return View();
        }

        public IActionResult RealizarPost()
        {
            return View();
        }

        public IActionResult RealizarComentario()
        {
            return View();
        }

        public IActionResult Reaccionar()
        {
            return View();
        }

        public IActionResult ListarPorTextoyNumero()
        {
            return View();
        }
    }
}
