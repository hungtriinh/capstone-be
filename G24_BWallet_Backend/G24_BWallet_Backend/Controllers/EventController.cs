﻿using G24_BWallet_Backend.Models;
using G24_BWallet_Backend.Models.ObjectType;
using G24_BWallet_Backend.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace G24_BWallet_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository repo;

        public EventController(IEventRepository eventRepository)
        {
            repo = eventRepository;
        }
        protected int GetUserId()
        {
            return int.Parse(this.User.Claims.First(i => i.Type == "UserId").Value);
        }

        [HttpGet]
        public async Task<Respond<IEnumerable<EventHome>>> GetAllEvent()
        {
            var events = repo.GetAllEventsAsync(GetUserId());
            return new Respond<IEnumerable<EventHome>>()
            {
                StatusCode = HttpStatusCode.Accepted,
                Error = "",
                Message = "Get event success",
                Data = await events
            };
        }

        //[HttpGet("{userID}")]
        //public async Task<Respond<IEnumerable<EventHome>>> GetAllEvent(int userID)
        //{
        //    var events = repo.GetAllEventsAsync(userID);
        //    return new Respond<IEnumerable<EventHome>>()
        //    {
        //        StatusCode = HttpStatusCode.Accepted,
        //        Error = "",
        //        Message = "Get event success",
        //        Data = await events
        //    };
        //}

        [HttpPost]
        public async Task<Respond<string>> AddEvent( NewEvent newEvent)
        {
            Event e = new Event
            {
                EventName = newEvent.EventName,
                EventDescript = newEvent.EventDescript,
                EventLogo = newEvent.EventLogo
            };
            int eventID = await repo.AddEventAsync(e);
            await repo.AddEventMember(eventID, newEvent.MemberIDs);
            string eventUrl = await repo.CreateEventUrl(eventID);
            return new Respond<string>()
            {
                StatusCode = HttpStatusCode.Accepted,
                Error = "",
                Message = "Add event success",
                Data = eventUrl
            };
        }

        [HttpPost("join/eventId={eventId}")]
        public async Task<Respond<IDictionary>> CheckJoinByUrl(int eventId)
        {
            EventUserID eu = new EventUserID { EventId = eventId, UserId = GetUserId() };
            bool isJoin = await repo.CheckUserJoinEvent(eu);
            IDictionary<string, int> result = new Dictionary<string, int>
            {
                { "EventId", eventId }
            };
            if (isJoin == false)
                return new Respond<IDictionary>()
                {
                    StatusCode = HttpStatusCode.NotAcceptable,
                    Error = "",
                    Message = "User chưa tham gia event",
                    Data = (IDictionary)result
                };
            return new Respond<IDictionary>()
            {
                StatusCode = HttpStatusCode.Accepted,
                Error = "",
                Message = "User đã tham gia event",
                Data = (IDictionary)result
            };
        }

        [HttpGet("ShareableLink/EventId={eventId}")]
        public async Task<Respond<string>> GetEventLink(int eventId)
        {
            string link = await repo.GetEventUrl(eventId);
            return new Respond<string>()
            {
                StatusCode = HttpStatusCode.Accepted,
                Error = "",
                Message = "Lấy link event đã tạo để share",
                Data = link
            };
        }

        [HttpGet("EventIntroduce/EventId={eventId}")]
        public async Task<Respond<IDictionary>> ShowEventIntroduce(int eventId)
        {
            Event e = await repo.GetEventIntroduce(eventId);
            List<UserAvatarName> u = await repo.GetListUserInEvent(eventId);
            IDictionary<string, object> result = new Dictionary<string, object>
            {
                { "EventLogo", e.EventLogo},
                {"EventName", e.EventName },
                {"EventDescript", e.EventDescript },
                {"TotalMembers", u.Count.ToString() },
                {"ListMembers", u }
            };
            return new Respond<IDictionary>()
            {
                StatusCode = HttpStatusCode.Accepted,
                Error = "",
                Message = "Lấy thông tin event và các thành viên để xin join",
                Data = (IDictionary)result
            };
        }

        [HttpPost("JoinRequest")]
        public async Task<Respond<string>> SendJoinRequest(EventUserID eventUserID)
        {
            eventUserID.UserId = GetUserId();
            var check = await repo.SendJoinRequest(eventUserID);
            if(check == false)
                return new Respond<string>()
                {
                    StatusCode = HttpStatusCode.NotAcceptable,
                    Error = "",
                    Message = "User đã ở trong event này rồi!",
                    Data = null
                };
            return new Respond<string>()
            {
                StatusCode = HttpStatusCode.Accepted,
                Error = "",
                Message = "User xin join vào Event thành công. Đang chờ duyệt!",
                Data = null
            };
        }
    }

}

