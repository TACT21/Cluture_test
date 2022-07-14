﻿using System;
using System.Collections.Generic;
namespace Test.Server
{
    public class IntegrationProvider
    {
        static private class Librarian
        {
            internal static Dictionary<string, string> core = new Dictionary<string, string>();
        }
        public class Provider
        {
            public async Task Register(string uuid,string email)
            {
                if (Librarian.core[uuid] != email)
                {
                    throw new Exception();
                }
            }
            public async Task CheckIn(string uuid, string email)
            {
                Librarian.core.Add(uuid, email);
            }
            public async Task CheckOut(string uuid, string email)
            {
                if(Librarian.core[uuid] == email)
                {
                    Librarian.core.Remove(uuid);
                }
            }
        }
    }
}
