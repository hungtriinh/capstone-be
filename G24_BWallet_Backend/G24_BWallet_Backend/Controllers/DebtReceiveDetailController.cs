﻿using G24_BWallet_Backend.Models.ObjectType;
using G24_BWallet_Backend.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace G24_BWallet_Backend.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DebtReceiveDetailController : ControllerBase
    {
        private readonly IDebtReceiveDetailRepo repo;

        public DebtReceiveDetailController(IDebtReceiveDetailRepo re)
        {
            this.repo = re;
        }

        protected int GetUserId()
        {
            return int.Parse(this.User.Claims.First(i => i.Type == "UserId").Value);
        }

        [HttpGet("getDebt/EventId={eventId}")]
        public async Task<Respond<TotalMoneyUser>> GetDebt(int eventId)
        {
            TotalMoneyUser result = await repo.GetAllDebtInEvent(GetUserId(), eventId);
            return new Respond<TotalMoneyUser>()
            {
                StatusCode = HttpStatusCode.Accepted,
                Error = "",
                Message = "Chi tiết khoản nợ của mình",
                Data = result
            };
        }

        [HttpGet("getReceive/EventId={eventId}")]
        public async Task<Respond<TotalMoneyUser>> GetReceive(int eventId)
        {
            TotalMoneyUser result = await repo.GetAllReceiveInEvent(GetUserId(), eventId);
            return new Respond<TotalMoneyUser>()
            {
                StatusCode = HttpStatusCode.Accepted,
                Error = "",
                Message = "Chi tiết khoản thu của mình",
                Data = result
            };
        }

        [HttpGet("showDetail/ReceipId={receiptId}")]
        public async Task<Respond<ReceiptUserDeptName>> ClickIButton(int receiptId)
        {
            ReceiptUserDeptName result = await repo.ClickIButton(receiptId, GetUserId());
            return new Respond<ReceiptUserDeptName>()
            {
                StatusCode = HttpStatusCode.Accepted,
                Error = "",
                Message = "Chi tiết chứng từ",
                Data = result
            };
        }
    }
}
