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
            public ulong points;
        }

        XmlDocument doc = new XmlDocument();
        public List<Member> members = new List<Member>();

        public PointService()
        {
            LoadPoints();
        }

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
                m.points = UInt64.Parse(points);

                members.Add(m);
            }
            Console.WriteLine("Members Loaded");
        }

        public void SavePoints()
        {
            Console.WriteLine("Saveing Points To File");
            doc.Load("Points.xml");
            foreach (Member m in members)
            {
                XmlNode node = MemberExist(m);
                if(node == null)
                {

                    return;
                }

                node.Attributes["points"].Value = m.points.ToString();

            }
        }

        public void AddMember(string name, ulong id, ulong points = 0)
        {
            if (MemberExist(id)) return;

            Member newMember = new Member();

            newMember.name = name;
            newMember.id = id;
            newMember.points = points;

            members.Add(newMember);
        }

        public void AddPoints(Member member, ulong points)
        {
            member.points += points;
        }

        private XmlNode MemberExist(Member m)
        {
            doc.Load("Points.xml");
            foreach(XmlNode node in doc.DocumentElement)
            {
                if (node.Attributes["id"].InnerText == m.id.ToString()) return node;
            }

            return null;
        }

        private bool MemberExist(ulong id)
        {
            doc.Load("Points.xml");
            foreach (XmlNode node in doc.DocumentElement)
            {
                if (node.Attributes["id"].InnerText == id.ToString()) return true;
            }
            return false;
        }

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
        
    }
}
