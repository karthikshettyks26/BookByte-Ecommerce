using BookByte.DataAccess.Repository.IRepository;
using BookByte.Models.Models;
using BookByte.Models.ViewModels;
using BookByte.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography.Xml;

namespace BookByte.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();

            return View(objCompanyList);
        }

        public IActionResult Upsert(int? id)
        {
            //Projection in EFCore - to pick selected value and not whole list
            #region commented for learning - ViewBag & ViewData
            //"View bag" is used to transfer data from controler tp View, not viceversa
            //It acts as Key-Value pair, here, Key - CategoryList & value - categoryList
            //"View Data" is also used for similar purpose. only difference is ViewData must be type casted before use.
            //ViewBag.CategoryList = categoryList;
            //ViewData["CategoryList"] = categoryList;
            #endregion

            if (id == null || id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //Update / edit
                Company companyObj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyObj);
            }

            
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                //create
                if (company.Id == null || company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                    TempData["Success"] = "Company created successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    TempData["Success"] = "Company updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(company);
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();

            return Json(new { data = objCompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion

    }
}
