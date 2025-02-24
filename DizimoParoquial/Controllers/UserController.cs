using DizimoParoquial.Models;
using Microsoft.AspNetCore.Mvc;

namespace DizimoParoquial.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SearchUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveUser(User usuario)
        {

            if (ModelState.IsValid)
            {
                // Aqui você pode salvar no banco de dados ou processar os dados
                return RedirectToAction("Sucesso");
            }
            return View(usuario); // Retorna a mesma view caso haja erros de validação

        }

        [HttpPost]
        public IActionResult EditUser()
        {
            return View();
        }
    }
}
