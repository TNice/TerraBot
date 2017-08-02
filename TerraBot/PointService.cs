using System;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TerraBot
{

    public class PointService
    {
        public struct Member
        {
            public string name;
            public ulong id;
            public double points;

            public Member(string name = "NULL", ulong id = 0, double points = 0)
            {
                this.name = name;
                this.id = id;
                this.points = points;
            }
        }

        XmlDocument doc = new XmlDocument();
        public List<Member> members = new List<Member>();

        public PointService()
        {
            LoadPoints();
        }

        /// <summary>
        /// Loads Points.xml into the members list
        /// </summary>
        public void LoadPoints()
        {       
            doc.Load("Points.xml");
            Console.WriteLine("Loading Members");
            foreach (XmlNode node in doc.DocumentElement)
            {
                Member m = new Member();

                string name = node.Attributes["name"].InnerText;
                string id = node.Attributes["id"].InnerText;         
                string points = node.Attributes["points"].InnerText;

                m.name = name;
                m.id = UInt64.Parse(id);
                m.points = Double.Parse(points);

                members.Add(m);
            }
            Console.WriteLine("Members Loaded");
        }

        /// <summary>
        /// Saves All Members To Points.xml
        /// </summary>
        public void SavePoints()
        {
            Console.WriteLine("Saveing Points To File");
            doc.Load("Points.xml");
            foreach (Member m in members)
            {
                XmlNode node = MemberExist(m);
                if(node == null)
                {
                    XmlElement element = doc.CreateElement("member");
                    element.SetAttribute("name", m.name);
                    element.SetAttribute("id", m.id.ToString());
                    element.SetAttribute("points", m.points.ToString());
                    return;
                }

                node.Attributes["points"].Value = m.points.ToString();

            }
        }

        /// <summary>
        /// Saves Only One Member To Points.xml
        /// </summary>
        /// <param name="member">Member Struct To Save</param>
        public void SavePoints(Member member)
        {
            XmlNode node = MemberExist(member);
            if(node == null)
            {
                XmlElement element = doc.CreateElement("member");
                element.SetAttribute("name", member.name);
                element.SetAttribute("id", member.id.ToString());
                element.SetAttribute("points", member.points.ToString());
                return;
            }

            node.Attributes["points"].Value = member.points.ToString();

        }

        /// <summary>
        /// Adds A Member To Members List. (Does Not Save To Points.xml
        /// </summary>
        /// <param name="name">Name of member(Nickname when passed by CommandModule)</param>
        /// <param name="id">Id of Member</param>
        /// <param name="points">Number of points the member has</param>
        public void AddMember(string name, ulong id, ulong points = 0)
        {
            if (MemberExist(id)) return;

            Member newMember = new Member();

            newMember.name = name;
            newMember.id = id;
            newMember.points = points;

            members.Add(newMember);
        }

        /// <summary>
        /// Adds All Members From A Server (Also Saves To xml)
        /// </summary>
        /// <param name="serv">Server to  add all mebers from. Ideally Passed By CommandModule</param>
        public void AddAllMembers(SocketGuild serv)
        {
            foreach(SocketGuildUser u in serv.Users)
            {
                if (FindMember(u.Id) != -1)
                    continue;

                Member newMem = new Member();
                newMem.name = u.Nickname;
                newMem.id = u.Id;
                newMem.points = 0;

                members.Add(newMem);
            }

            SavePoints();
        }

        /// <summary>
        /// Gives a member points
        /// </summary>
        /// <param name="member">Member to give points(use GetMember to pass a member through)</param>
        /// <param name="points">Number of Points To Get</param>
        public void AddPoints(Member member, double points)
        {
            member.points += points;
        }

        /// <summary>
        /// Checks If A Member Exists
        /// </summary>
        /// <param name="m">Memeber as a struct Member</param>
        /// <returns>Node Of Member</returns>
        private XmlNode MemberExist(Member m)
        {
            doc.Load("Points.xml");
            foreach(XmlNode node in doc.DocumentElement)
            {
                if (node.Attributes["id"].InnerText == m.id.ToString()) return node;
            }

            return null;
        }

        /// <summary>
        /// Checks If A Member Exists
        /// </summary>
        /// <param name="id">Id of member to search</param>
        /// <returns>true if member is found</returns>
        private bool MemberExist(ulong id)
        {
            doc.Load("Points.xml");
            foreach (XmlNode node in doc.DocumentElement)
            {
                if (node.Attributes["id"].InnerText == id.ToString()) return true;
            }
            return false;
        }

        /// <summary>
        /// Finds index of member in member list
        /// </summary>
        /// <param name="id">Member id to search</param>
        /// <returns>integer index of member in member list</returns>
        public int FindMember(ulong id)
        {
            int count = 0;
            foreach (Member m in members)
            {
                if (m.id == id) return count;
                count++;
            }

            return -1;
        }

        /// <summary>
        /// Gets A Member from member list
        /// </summary>
        /// <param name="id">id of member to search</param>
        /// <returns>Member struct from members list</returns>
        public Member GetMember(ulong id)
        {
            foreach(Member m in members)
            {
                if (m.id == id)
                    return m;
            }

            return new Member();
        }
        
    }
}
