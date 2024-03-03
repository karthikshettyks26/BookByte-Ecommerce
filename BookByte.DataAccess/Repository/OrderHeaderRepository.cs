using BookByte.DataAccess.Data;
using BookByte.DataAccess.Repository.IRepository;
using BookByte.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookByte.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

		void IOrderHeaderRepository.Update(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader);
        }

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if(orderHeaderFromDb != null)
			{
				orderHeaderFromDb.OrderStatus = orderStatus;
				if (!string.IsNullOrEmpty(paymentStatus))
				{
					orderHeaderFromDb.PaymentStatus = paymentStatus; 
				}
			}
		}

		public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			var orderHeaderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if(!string.IsNullOrEmpty(sessionId))
			{
				orderHeaderFromDb.SessionId = sessionId;
			}

			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				orderHeaderFromDb.PaymentIntentId = paymentIntentId;
				orderHeaderFromDb.PaymentDate = DateTime.Now;
			}
		}
	}
}
