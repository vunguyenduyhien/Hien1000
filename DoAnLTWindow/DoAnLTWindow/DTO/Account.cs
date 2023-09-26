﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoAnLTWindow.DTO
{
    public class Account
    {
        public Account(string username, string displayname, int type, string pass = null)
        {
            Username = username;
            Displayname = displayname;
            Type = type;
            Pass = pass;
        }
        public Account(DataRow row)
        {
            Username = row["username"].ToString();
            Displayname = row["displayname"].ToString();
            Type = (int)row["type"];
            Pass = row["pass"].ToString();
        }
        private int type;
        private string pass;
        private string displayname;
        private string username;

        public string Username { get => username; set => username = value; }
        public string Displayname { get => displayname; set => displayname = value; }
        public string Pass { get => pass; set => pass = value; }
        public int Type { get => type; set => type = value; }
    }
}
