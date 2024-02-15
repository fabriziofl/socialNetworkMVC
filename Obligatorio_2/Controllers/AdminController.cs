using Logica_De_Negocio;
using Microsoft.AspNetCore.Mvc;

namespace Obligatorio_2.Controllers
{
    public class AdminController : Controller
    {
        private Sistema _miSistema = Sistema.Instancia;

        public IActionResult ListarUsuarios()
        {
            ViewBag.Usuarios = _miSistema.DevolverMiembros();

            return View();
        }

        public IActionResult BloquearUsuario()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BloquearUsuario(string email)
        {
            if (_miSistema.BuscarUsuario(email) != null && _miSistema.BuscarUsuario(email) is Miembro)
            {
                Miembro usuario = (Miembro)_miSistema.BuscarUsuario(email);

                usuario.CambiarEstado(true);

                ViewBag.SuccessMessage = "Usuario Bloqueado";
                return View();

            }

            ViewBag.ErrorMessage = "Usuario No Existe";
            return View();
        }


        public IActionResult BloquearPost()
        {
            ViewBag.Posts = _miSistema.DevolverPost();
            
            return View();
        }
    
      

    }
}
