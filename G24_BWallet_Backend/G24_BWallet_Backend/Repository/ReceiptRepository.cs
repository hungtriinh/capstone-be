﻿using G24_BWallet_Backend.DBContexts;
using G24_BWallet_Backend.Models;
using G24_BWallet_Backend.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace G24_BWallet_Backend.Repository
{
    public class ReceiptRepository :IReceiptRepository
    {
        private readonly MyDBContext myDB;

        public ReceiptRepository(MyDBContext myDB)
        {
            this.myDB = myDB;
        }

        public async Task<bool> AddReceiptAsync(Receipt addReceipt)//
        {
            myDB.Receipts.Add(addReceipt);
            return true;
        }

        public async Task<Receipt> GetReceiptByIDAsync (int ReceiptID)//
        {
            Receipt r = myDB.Receipts.Include(r => r.UserID).FirstOrDefault(x => x.ReceiptID == ReceiptID);
            return r;
        }

        public async Task<List<Receipt>> GetReceiptByEventIDAsync(int EventID)//
        {
            List<Receipt> receiptList = await myDB.Receipts//.Include(u => u.User).Include(u => u.User)
                .Where(e => e.EventID == EventID)
                .ToListAsync();
            return receiptList;
        }
    }
}
