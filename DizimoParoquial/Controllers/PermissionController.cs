using DizimoParoquial.DTOs;
using DizimoParoquial.Exceptions;
using DizimoParoquial.Models;
using DizimoParoquial.Services;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System.Text.Json;

namespace DizimoParoquial.Controllers
{
    public class PermissionController : Controller
    {

        private const string ROUTE_SCREEN_PERMISSIONS = "/Views/Permission/Index.cshtml"; 

        private readonly IToastNotification _notification;
        private readonly PermissionService _permissionService;

        public PermissionController(IToastNotification notification, PermissionService permissionService)
        {
            _notification = notification;
            _permissionService = permissionService;
        }

        public IActionResult Index()
        {
            ViewBag.SelectedUserId = 0;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchPermissions(int user)
        {

            if(user == 0)
            {
                _notification.AddErrorToastMessage("É preciso escolher um usuário para alterar as permissões!");
                return View(ROUTE_SCREEN_PERMISSIONS);
            }

            List<UserPermissionDTO> userPermissions = await _permissionService.GetUserPermissions(user);

            if(userPermissions == null || userPermissions.Count == 0)
                _notification.AddWarningToastMessage("Usuário não possui permissões cadastradas!");

            ViewBag.SelectedUserId = user;

            return View(ROUTE_SCREEN_PERMISSIONS, userPermissions);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePermissions(int userId, List<int> selectedPermissions)
        {

            try
            {
                if (userId == 0 || selectedPermissions == null || selectedPermissions.Count == 0)
                {
                    _notification.AddErrorToastMessage("É preciso escolher um usuário e ao menos uma permissão para salvar!");
                    return View(ROUTE_SCREEN_PERMISSIONS);
                }

                bool permissionsWereRegistered = await _permissionService.UpdatePermissions(userId, selectedPermissions);

                if (permissionsWereRegistered)
                    _notification.AddSuccessToastMessage("Permissões salvas com sucesso!");
                else
                    _notification.AddErrorToastMessage("Não foi possível salvar as permissões!");
            }
            catch (ValidationException ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }
            catch (NullException ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }
            catch (RepositoryException ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }
            catch (Exception ex)
            {
                _notification.AddErrorToastMessage(ex.Message);
            }

            return RedirectToAction(nameof(Index));

        }

    }
}
