using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace TerraBot
{
    class CommandModule : ModuleBase
    {
        [Command("ping"), Summary("Pings The Bot")]
        public async Task PingCommand()
        {
            await ReplyAsync("pong!");
        }
    }
}
