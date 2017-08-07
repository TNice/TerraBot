﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace TerraBot.Services
{

    public static class MemberService
    {
        [Serializable()]
        public class Member
        {
            public string name;
            public ulong id;
            public double points;
            public ulong server;

            public int tickets; //Will Be Used For A Lotto
            public int voteWeight; //Poll will favor more active members

            public Member(string name = "NULL", ulong id = 0, double points = 0, ulong server = 0)
            {
                this.name = name;
                this.id = id;
                this.points = points;
                this.server = server;
            }
        }

        private static List<Member> mList = new List<Member>();
        private static Dictionary<string, double> ranks = new Dictionary<string, double>();

        /// <summary>
        /// Gets Size Of Member List
        /// </summary>
        /// <returns>Size Of mList</returns>
        public static int ListSize()
        {
            return mList.Count;
        }

        /// <summary>
        /// Saves Memory To Settings File
        /// </summary>
        public static void SaveMembers()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, mList);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.Members = Convert.ToBase64String(buffer);
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Loads mList From Settings
        /// </summary>
        public static void LoadMembers()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.Members)))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    mList = (List<Member>)bf.Deserialize(ms);
                    Console.WriteLine("Members Loaded");
                }
            }
            catch(Exception)
            {
                Console.WriteLine("Members Unable To Load");
            }
        }

        //Need to make the url universal
        /// <summary>
        /// Loads Ranks From Ranks.txt
        /// </summary>
        public static void LoadRanks()
        {
            try
            {
                using (StreamReader read = File.OpenText(Properties.Settings.Default.RanksDoc))
                {
                    while (!read.EndOfStream)
                    {
                        List<string> line = read.ReadLine().Split(' ').ToList();
                        double val;
                        if (Double.TryParse(line[line.Count - 1], out val))
                        {
                            string key = "";
                            line.RemoveAt(line.Count - 1);
                            for (int i = 0; i < line.Count; i++)
                            {
                                if (i == 0)
                                    key = line[i];
                                else
                                    key += $" {line[i]}";
                            }

                            if (key != "")
                                ranks.Add(key, val);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Ranks.txt Not Found Check File Path! The Settings.settings file can be edited with notepad++");
            }
            Console.WriteLine("Ranks Loaded");
        }

        /// <summary>
        /// Prints Out All Ranks In Dictonaru To Console
        /// </summary>
        public static void PrintRanks()
        {
            foreach(var r in ranks)
            {
                Console.WriteLine($"Rank {r.Key} requires {r.Value}");
            }
        }

        /// <summary>
        /// Checks if member exist
        /// </summary>
        /// <param name="mem"></param>
        /// <returns></returns>
        public static bool MemberExists(Member mem)
        {
            foreach(var m in mList)
            {
                if (m.id == mem.id && m.server == mem.server)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Creates New Member Object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="points"></param>
        /// <param name="server">server id (used if when connected to multiple servers)</param>
        /// <returns></returns>
        public static Member CreateMember(string name, ulong id, double points = 0, ulong server = 0)
        {
            return new Member(name, id, points, server);
        }

        /// <summary>
        /// Removes Member From mList
        /// </summary>
        /// <param name="i"></param>
        public static void RemoveMember(int i)
        {
            mList.RemoveAt(i);
        }

        /// <summary>
        /// Add Member To Point Service List
        /// </summary>
        /// <param name="m">Use CreateMember Funtion For Ease Of Use</param>
        public static void AddMember(Member m)
        {
            mList.Add(m);
        }

        /// <summary>
        /// Used To Find Member In List For Single Server Use
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Index Of Member In List</returns>
        public static int FindMember(ulong id)
        {
            int count = 0;
            foreach(var m in mList)
            {
                if (id == m.id)
                    return count;
                count++;
            }
            return -1;
        }

        /// <summary>
        /// Used To Find Member In List For Muli Server Use
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Index Of Member In List</returns>
        public static int FindMember(ulong id, ulong server)
        {
            int count = 0;
            foreach (var m in mList)
            {
                if (id == m.id && server == m.server)
                    return count;
                count++;
            }
            return -1;
        }

        /// <summary>
        /// Adds Points To Member
        /// </summary>
        /// <param name="i">Index Of User In List</param>
        /// <param name="points"></param>
        /// <returns>Success Of Adding Point</returns>
        public static bool AddPoints(int i, double points)
        {
            if(i < 0 || i > mList.Count)
            {
                return false;
            }
            mList[i].points += points;
            return true;
        }

        public static void SortByPoints()
        {

        }

        /// <summary>
        /// Makes A New List Of Members In A Specified Server
        /// </summary>
        /// <param name="server">Server Id</param>
        /// <returns>List Of All Members In Server</returns>
        public static List<Member> GetMembersInServer(ulong server)
        {
            List<Member> tList = new List<Member>();
            foreach(var m in mList)
            {
                if (m.server == server)
                    tList.Add(m);
            }
            return tList;
        }

        /// <summary>
        /// Controlls brodcast of usernames and points to discord
        /// </summary>
        public static class SendPointsToClient
        {
            /// <summary>
            /// Sends List Of All Members Points To The Server
            /// </summary>
            /// <param name="server"></param>
            /// <returns>List of usersnames and their points as a string</returns>
            public static List<string> All(ulong server)
            {
                List<string> lines = new List<string>();
                foreach (var m in GetMembersInServer(server))
                {
                    lines.Add($"{m.name} - {m.points}");
                }
                return lines;
            }

            /// <summary>
            /// Sends A String That Combines Username and Points To The Client
            /// </summary>
            /// <param name="id"></param>
            /// <param name="server"></param>
            /// <returns>String containing points and username</returns>
            public static string Single(ulong id, ulong server)
            {
                string msg = "";
                foreach(var m in mList)
                {
                    if (m.id == id && m.server == server)
                        msg = $"{m.name} - {m.points}";
                }
                return msg;
            }
        }

        /// <summary>
        /// Finds Member's Discord User Id from there user name
        /// </summary>
        public static class GetMemberIdFromName
        {
            /// <summary>
            /// Gets The First Id Found In A Server Or In The List If Server Is Omitted in call
            /// </summary>
            /// <param name="name">Username to look up</param>
            /// <param name="server">Optional server id</param>
            /// <returns>single user id</returns>
            public static ulong FirstOrDefault(string name, ulong server = 0)
            {
                ulong id = 0;
                if(server == 0)
                {
                    foreach(var m in mList)
                    {
                        if (m.name == name)
                            id = m.id;
                    }
                }
                else
                    foreach(var m in mList)
                    {
                        if (m.name == name && m.server == server)
                            id = m.id;
                    }
                return id;
            }

            /// <summary>
            /// Finds All User IDs in List (NOTE* only use this if there is a reason to believe 2 seperate members have same username)
            /// </summary>
            /// <param name="name">user name</param>
            /// <returns>All User IDs in a list that corrispond to the username</returns>
            public static List<ulong> All(string name)
            {
                List<ulong> list = new List<ulong>();
                foreach(var m in mList)
                {
                    if (m.name == name)
                        list.Add(m.id);
                }

                return list;
            }
        }

        public static void SetVoteWeight()
        {
            
        }

        /// <summary>
        /// Prints Member List To Console
        /// </summary>
        public static void PrintList()
        {
            foreach(var m in mList)
            {
                Console.WriteLine("Name: {0} Id: {1} ServerId: {2} Points: {3}", m.name, m.id, m.server, m.points);
            }
        }

    }
}
