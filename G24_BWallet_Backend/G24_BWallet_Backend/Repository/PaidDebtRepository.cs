﻿using G24_BWallet_Backend.DBContexts;
using G24_BWallet_Backend.Models;
using G24_BWallet_Backend.Models.ObjectType;
using G24_BWallet_Backend.Repository;
using G24_BWallet_Backend.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace G24_BWallet_Backend.Repository
{
    public class PaidDebtRepository : IPaidDebtRepository
    {
        private readonly MyDBContext context;

        public PaidDebtRepository(MyDBContext myDB)
        {
            this.context = myDB;
        }
        public async Task<List<Receipt>> GetReceipts(int eventId, int status)
        {
            var list = context.Receipts.Include(r => r.UserDepts).Include(r => r.User)
                .Where(r => r.EventID == eventId && r.ReceiptStatus == status)
                .OrderByDescending(r => r.Id)
                .ToListAsync();
            return await list;
        }

        public async Task<List<UserDebtReturn>> GetUserDepts(List<Receipt> receipt, int userId)
        {
            List<UserDebtReturn> userDepts = new List<UserDebtReturn>();
            foreach (var item in receipt)
            {
                UserDept ud = item.UserDepts.Where(ud => ud.UserId == userId).FirstOrDefault();
                if (ud != null)
                {
                    UserDebtReturn udr = new UserDebtReturn();
                    udr.UserDeptId = ud.Id;
                    udr.ReceiptName = item.ReceiptName;
                    udr.Date = item.CreatedAt + "";
                    udr.OwnerName = item.User.UserName;
                    udr.DebtLeft = ud.DebtLeft;
                    userDepts.Add(udr);
                }

            }
            userDepts.Reverse();
            return await Task.FromResult(userDepts);
        }

        public async Task<PaidDept> PaidDebtInEvent(PaidDebtParam p)//create paid dept
        {
            DateTime VNDateTimeNow = TimeZoneInfo
                .ConvertTime(DateTime.Now,
                TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
            PaidDept paidDept = new PaidDept
            {
                UserId = p.UserId,
                EventId = p.EventId,
                TotalMoney = p.TotalMoney,
                Status = 1,
                Code = p.Code,
                Type = p.Type,
                UpdatedAt = VNDateTimeNow,
                CreatedAt = VNDateTimeNow
            };
            //try
            //{
            await context.PaidDepts.AddAsync(paidDept);
            await context.SaveChangesAsync();
            foreach (var item in p.ListEachPaidDebt)
            {
                var check = paidDept.Id;
                PaidDebtList paid = new PaidDebtList
                {
                    PaidId = paidDept.Id,
                    DebtId = item.userDeptId,
                    PaidAmount = item.debtLeft
                };
                await context.PaidDebtLists.AddAsync(paid);
                await context.SaveChangesAsync();
                await ChangeDebtLeft(item);
            }
           
            //}
            //catch (Exception e)
            //{
            //    throw new Exception("PaidDept:Lỗi ghi tiền trả");
            //}
            return paidDept;
        }

        private async Task ChangeDebtLeft(RenamePaidDebtList item)
        {
            var paiddlist = new PaidDebtList
            {
                DebtId = item.userDeptId,
                PaidAmount = item.debtLeft
            };
            var userDebt = await context.UserDepts.FirstOrDefaultAsync(u => u.Id == paiddlist.DebtId);
            userDebt.DebtLeft -= paiddlist.PaidAmount;
            if (userDebt.DebtLeft <= 0)// tra het no
            {
                userDebt.DeptStatus = 0;
            }
            await context.SaveChangesAsync();
        }
        public async Task<List<DebtPaymentPending>> PaidDebtRequestSent(int userId, int eventId)
        {
            List<DebtPaymentPending> result = new List<DebtPaymentPending>();
            List<PaidDept> paidDepts = await context.PaidDepts
                .Where(p => p.EventId == eventId && p.UserId == userId).ToListAsync();
            foreach (PaidDept item in paidDepts)
            {
                DebtPaymentPending debtPayment = new DebtPaymentPending();
                debtPayment.TotalMoney = item.TotalMoney;
                debtPayment.Date = item.CreatedAt.ToString();
                debtPayment.Code = item.Code;
                debtPayment.ImageLink = await context.ProofImages
                    .Where(p => p.ImageType.Equals("paidDept") && p.ModelId == item.Id)
                    .Select(p => p.ImageLink).FirstOrDefaultAsync();
                debtPayment.Type = item.Type;
                debtPayment.Status = item.Status;
                User cashier = await GetCashier(eventId);
                debtPayment.cashier = new UserAvatarName
                { Avatar = cashier.Avatar, Name = cashier.UserName };
                result.Add(debtPayment);
            }
            return result;
        }

        private async Task<User> GetCashier(int eventId)
        {
            EventUser cashier = await context.EventUsers.Include(e => e.User)
                .FirstOrDefaultAsync(u => u.EventID == eventId && u.UserRole == 3);
            EventUser owner = await context.EventUsers.Include(e => e.User)
                .FirstOrDefaultAsync(u => u.EventID == eventId && u.UserRole == 1);
            EventUser inspector = await context.EventUsers.Include(e => e.User)
                .FirstOrDefaultAsync(u => u.EventID == eventId && u.UserRole == 2);
            if (cashier != null) return cashier.User;
            else if (owner != null) return owner.User;
            return inspector.User;
        }
    }
}
