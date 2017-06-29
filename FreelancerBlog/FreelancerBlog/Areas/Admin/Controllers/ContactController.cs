using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cloudscribe.Web.Pagination;
using FreelancerBlog.Core.Repository;
using FreelancerBlog.Mapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreelancerBlog.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ContactController : Controller
    {
        private IUnitOfWork _uw;
        private IFreelancerBlogMapper _freelancerBlogMapper;

        public ContactController(IUnitOfWork uw, IFreelancerBlogMapper freelancerBlogMapper)
        {
            _uw = uw;
            _freelancerBlogMapper = freelancerBlogMapper;
        }

        [HttpGet]
        public async Task<IActionResult> ManageContact(int? page)
        {
            var contacts = await _uw.ContactRepository.GetAllAsync();

            var contactsViewModel = _freelancerBlogMapper.ContactCollectionToContactViewModelCollection(contacts).Take(2).ToList();

            contactsViewModel.ForEach(c => c.ContactText = c.ContactId + " - Numb");
            contactsViewModel.ForEach(c => c.ContactDropSelectListItems =
                new List<SelectListItem>
                {
                    new SelectListItem {Text = 1.ToString(), Value = 1.ToString(), Selected = true},
                    new SelectListItem {Text = 2.ToString(), Value = 2.ToString()}
                });
            contactsViewModel.ForEach(c => c.Price = 0);
            contactsViewModel.ForEach(c => c.Quantity = 1);

            var pageNumber = page ?? 1;

            var pagedContact = contactsViewModel.ToPagedList(pageNumber - 1, 20);

            return View(pagedContact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteContact(int id)
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

            int deleteContactResult = await _uw.ContactRepository.DeleteContactAsync(model);

            if (deleteContactResult > 0)
            {
                return Json(new { Status = "Deleted" });
            }

            return Json(new { Status = "NotDeletedSomeProblem" });
        }

        protected override void Dispose(bool disposing)
        {
            _uw.Dispose();
            base.Dispose(disposing);
        }
    }
}