using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PFSign.Models;

namespace PFSign.Controllers
{
    [Route("/api/[Controller]")]
    public class RecordController
    {
        /// <summary>查询Api</summary>
        /// <param name="begin">开始日期</param>
        /// <param name="end">结束日期</param>
        /// <returns>[{Name, Seat, SignInTime, SignOutTime}]</returns>
        public object Index
            (DateTime? begin, DateTime? end)
        {
            DateTime endTime = end ?? DateTime.UtcNow;
            DateTime beginTime = begin ?? endTime.AddDays(-1);
            return (from r in Records
                    where r.SignOutTime == null
                    && r.SignInTime >= beginTime
                    && r.SignInTime <= endTime
                    select new
                    {
                        Name        = r.Name,
                        Seat        = r.Seat,
                        SignInTime  = r.SignInTime,
                        SignOutTime = r.SignOutTime,
                    }).ToList();
        }

        /// <summary>签到Api</summary>
        /// <param name="userId">用户的标识Id</param>
        /// <param name="name">用户的显示的名称</param>
        /// <param name="seat">座位编号</param>
        /// <returns>[{result, msg}]</returns>
        [HttpPost("[Action]")]
        public object SignIn
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
            Records.Add(record);

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
        public object SignOut(string userId)
        {
            Record record = (from r in Records
                             where r.SignOutTime == null
                             && r.UserId == userId
                             select r).FirstOrDefault();
            
            if(record == null)
            {
                return new
                {
                    result = false,
                    msg    = "未能找到对于记录！"
                };
            }
            
            record.SignOutTime = DateTime.UtcNow;

            return new
            {
                result = true,
                msg    = ""
            };
        }

        private static List<Record> Records = 
            new List<Record>()
        {
            new Record()
            {
                Id = Guid.NewGuid(),
                UserId = "001",
                Name = "Test1",
                SignInTime = DateTime.Parse("2018-01-24"),
                SignOutTime = DateTime.Parse("2018-01-25"),
                Seat = 1
            },
            new Record()
            {
                Id = Guid.NewGuid(),
                UserId = "002",
                Name = "Test2",
                SignInTime = DateTime.Parse("2018-01-24"),
                Seat = 2
            }
        };

    }
}