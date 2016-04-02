using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using WebFor.Core.Domain;
using WebFor.Core.Repository;
using WebFor.Infrastructure.EntityFramework;
using WebFor.Web.Services;
using WebFor.Web.ViewModels.Contact;
using cloudscribe.Web.Pagination;
using WebFor.Web.Areas.Admin.ViewModels.Portfolio;

namespace WebFor.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PortfolioController : Controller
    {
        private IUnitOfWork _uw;
        private IWebForMapper _webForMapper;

        public PortfolioController(IUnitOfWork uw, IWebForMapper webForMapper)
        {
            _uw = uw;
            _webForMapper = webForMapper;
        }

        [HttpGet]
        public async Task<IActionResult> ManagePortfolio(int? page)
        {
            var contacts = await _uw.ContactRepository.GetAllAsync();

            var contactsViewModel = _webForMapper.ContactCollectionToContactViewModelCollection(contacts);

            var pageNumber = page ?? 1;

            var pagedContact = contactsViewModel.ToPagedList(pageNumber - 1, 2);

            return View(pagedContact);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Create(PortfolioViewModel viewModel)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            if (id == default(int))
            {
                return Json(new { Status = "IdCannotBeNull" });
            }

            var model = await _uw.ContactRepository.FindByIdAsync(id);

            if (model == null)
            {
                return Json(new { Status = "ContactNotFound" });
            }

            try
            {
                int deleteContactResult = await _uw.ContactRepository.DeleteContactAsync(model);

                if (deleteContactResult > 0)
                {
                    return Json(new { Status = "Deleted" });
                }

                return Json(new { Status = "NotDeletedSomeProblem" });
            }

            catch (Exception eX)
            {
                return Json(new { Status = "Error", eXMessage = eX.Message });
            }
        }
    }
}
