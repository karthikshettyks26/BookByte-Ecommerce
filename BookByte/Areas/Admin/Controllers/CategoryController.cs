using BookByte.DataAccess.Data;
using BookByte.DataAccess.Repository.IRepository;
using BookByte.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookByte.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork db)
        {
            _unitOfWork = db;
        }

        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            //Custom Validation
            if(obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order cannot exactly match the Name.");
            }
            if (obj.Name != null && obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "Test is an invalid value.");
            }
            //Server side validation
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["Success"] = "Category created successfully!";
                return RedirectToAction("Index");
            }
            
            return View(obj);
            
        }

        public IActionResult Edit(int? id)
        {
            if(id== null || id == 0)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.Category.Get(u => u.Id == id);
            //Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id==id);
            //Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (category == null) 
            { 
                return NotFound(); 
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            //Custom Validation
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order cannot exactly match the Name.");
            }
            if (obj.Name != null && obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("", "Test is an invalid value.");
            }
            //Server side validation
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                //temporary pop up message-> added in next render page
                TempData["Success"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }

            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.Category.Get(u => u.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (obj== null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["Success"] = "Category deleted successfully!";
            return RedirectToAction("Index");
        }



    }
}
