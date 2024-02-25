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
    }
}
