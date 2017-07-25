using System;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraBot
{

    public class Points
    {
        public struct Member
        {
            public string name;
            public ulong id;
            public ulong points;
        }

        public List<Member> members = new List<Member>();

        public void LoadPoints()
        {
            //read config.xml and load data into list
        }

        public void SavePoints()
        {
            //Rewrite the config.xml file
        }

        public void AddMember(string name, ulong id, ulong points = 0)
        {
            Member newMember;

            newMember.name = name;
            newMember.id = id;
            newMember.points = points;

            members.Add(newMember);
        }

        public void AddPoints(ref Member member, ulong points)
        {
            member.points += points;
        }

        public Member FindMember(ulong id)
        {
            //Simple Search Alg For User In List
            return new Member();
        }
        
    }
}
