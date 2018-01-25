using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PFSign.Models;
using PFSign.Data;

namespace PFSign.Controllers
{
    [Route("/api/[Controller]")]
    public class RecordController
    {
        private readonly RecordDbContext _context;

        public RecordController(RecordDbContext context)
        {
            _context = context;
        }

        /// <summary>查询Api</summary>
        /// <param name="begin">开始日期</param>
        /// <param name="end">结束日期</param>
        /// <returns>[{Name, Seat, SignInTime, SignOutTime}]</returns>
        public async Task<object> Index
            (DateTime? begin, DateTime? end)
        {
            DateTime endTime = end ?? DateTime.UtcNow;
            DateTime beginTime = begin ?? endTime.AddDays(-1);
            return await (from r in _context.Records
                          where r.SignInTime >= beginTime
                          && r.SignInTime <= endTime
                          select new
                          {
                              Name        = r.Name,
                              Seat        = r.Seat,
                              SignInTime  = r.SignInTime,
                              SignOutTime = r.SignOutTime,
                          }).ToListAsync();
        }

        /// <summary>签到Api</summary>
        /// <param name="userId">用户的标识Id</param>
        /// <param name="name">用户的显示的名称</param>
        /// <param name="seat">座位编号</param>
        /// <returns>[{result, msg}]</returns>
        [HttpPost("[Action]")]
        public async Task<object> SignIn
            (string userId, string name, int seat)
        {
            Record record = new Record()
            {
                Id         = Guid.NewGuid(),
                UserId     = userId,
                Name       = name,
                SignInTime = DateTime.UtcNow,
                Seat       = seat
            };

            _context.Add(record);
            await _context.SaveChangesAsync();

            return new
            {
                result = true,
                msg    = ""
            };
        }

        /// <summary>签退Api</summary>
        /// <param name="userId">用户的标识Id</param>
        /// <returns>[{result, msg}]</returns>
        [HttpPost("[Action]")]
        public async Task<object> SignOut(string userId)
        {
            Record record = await (from r in _context.Records
                                   where r.SignOutTime == null
                                   && r.UserId == userId
                                   select r).FirstOrDefaultAsync();
            
            if(record == null)
            {
                return new
                {
                    result = false,
                    msg    = "未能找到对于记录！"
                };
            }
            
            record.SignOutTime = DateTime.UtcNow;

            _context.Update(record);
            await _context.SaveChangesAsync();

            return new
            {
                result = true,
                msg    = ""
            };
        }
    }
}