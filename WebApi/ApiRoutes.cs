﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace WebApi
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = Root + "/" + Version;

        public static class Books
        {
            public const string GetAll = Base + "/book";
            public const string Update = Base + "/book/{id}";
            public const string Delete = Base + "/book/{id}";
            public const string Get = Base + "/book/{id}";
            public const string Create = Base + "/book";
        }
        public static class Identity
        {
            public const string Login = Base + "/identity/login";
            public const string Register = Base + "/identity/register";
        }
    }
}
