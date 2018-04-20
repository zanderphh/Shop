﻿using Microsoft.AspNetCore.Http;
using System;

namespace Shop.AdminApis.Services
{
    public interface IContextService
    {
        AdminAccountIdentity GetCurrentAdmin(HttpContext httpContext);
    }

    public class AdminAccountIdentity
    {
        public string AdminId { get; private set; }
        public string Name { get; private set; }

        public AdminAccountIdentity(string adminId, string name)
        {
            AdminId = adminId;
            Name = name;
        }
    }
}