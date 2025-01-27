﻿using G24_BWallet_Backend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading.Tasks;
using G24_BWallet_Backend.Models.ObjectType;

namespace G24_BWallet_Backend.Repository.Interface
{
    public interface IEventUserRepository
    {
        Task<List<Member>> SearchEventUsersAsync(int eventID,int userID, string name = null);
        Task<int> GetEventUserRoleAsync(int eventID, int userID);
    }
}
