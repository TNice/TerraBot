using System;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace TerraBot
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

            public Member(string name = "NULL", ulong id = 0, double points = 0, ulong server = 0)
            {
                this.name = name;
                this.id = id;
                this.points = points;
                this.server = server;
            }
        }

        private static List<Member> mList = new List<Member>();

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
