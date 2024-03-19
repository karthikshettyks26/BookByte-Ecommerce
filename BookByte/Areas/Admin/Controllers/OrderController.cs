using BookByte.DataAccess.Repository.IRepository;
using BookByte.Models.Models;
using BookByte.Models.ViewModels;
using BookByte.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BookByte.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderVM OrderVM { get; set; }

		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int orderId)
		{
			OrderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};
			return View(OrderVM);
		}

		[HttpPost]
		[Authorize(Roles =SD.Role_Admin + "," + SD.Role_Employee)]
		public IActionResult UpdateOrderDetail()
		{
			var orderHeaderFromDB = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
			if(orderHeaderFromDB != null)
			{
				orderHeaderFromDB.Name = OrderVM.OrderHeader.Name;
                orderHeaderFromDB.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
                orderHeaderFromDB.StreetAddress = OrderVM.OrderHeader.StreetAddress;
                orderHeaderFromDB.City = OrderVM.OrderHeader.City;
                orderHeaderFromDB.State = OrderVM.OrderHeader.State;
                orderHeaderFromDB.PostalCode = OrderVM.OrderHeader.PostalCode;
				if (!string.IsNullOrEmpty(orderHeaderFromDB.Carrier))
					orderHeaderFromDB.Carrier = OrderVM.OrderHeader.Carrier;
                if (!string.IsNullOrEmpty(orderHeaderFromDB.TrackingNumber))
                    orderHeaderFromDB.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;

				_unitOfWork.OrderHeader.Update(orderHeaderFromDB);
				_unitOfWork.Save();

				TempData["Success"] = "Order Details Updated Successfully";
			}
			else
			{

			}
			return RedirectToAction(nameof(Details), new {orderId = OrderVM.OrderHeader.Id});
		}

        #region API CALLS
        [HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> objOrderHeaders;


            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
			{
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
			}
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

				objOrderHeaders = _unitOfWork.OrderHeader.GetAll(u=>u.ApplicationUserId == userId, includeProperties: "ApplicationUser").ToList();
            }


			switch (status)
			{
                case "pending":
					objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }

			return Json(new { data = objOrderHeaders });
		}


		#endregion
	}
}
