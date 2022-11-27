﻿using G24_BWallet_Backend.Models;
using G24_BWallet_Backend.Models.ObjectType;
using System.Threading.Tasks;

namespace G24_BWallet_Backend.Repository.Interface
{
    public interface IMemberRepository
    {
        Task<MemberDetailParam> ShowMemeberDetail(int eventId);
        Task<Event> GetEvent(int eventId);
        Task PromoteMemberRole(EventUserIDRole e);
        Task DeletePromoteMemberRole(EventUserID e);
        Task<bool> IsOwner(int eventId, int v);
        Task RemoveMember(EventUserID e);
    }
}
