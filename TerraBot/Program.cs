using System;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace TerraBot
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        public async Task MainAsync()
        {
            //Found in Settings.settings file
            string token = Settings.Default.Token;

            //Initalize and Setup Client and Services
            var config = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 100
            };
            client = new DiscordSocketClient(config);
            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();
            client.Log += Log;
            await InstallCommands();
          
            //Add Event Overides Here
            client.MessageReceived += MessageRecieved;
            client.MessageUpdated += MessageUpdated;
            client.Ready += () =>
            {
                Console.WriteLine("Bot Connected!");
                return Task.CompletedTask;
            };

            //Login
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();


            await Task.Delay(-1);
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {after}");
        }

        private async Task MessageRecieved(SocketMessage message)
        {
            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Pong");
            }
           
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandelCommands;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandelCommands(SocketMessage msgParam)
        {
            var message = msgParam as SocketUserMessage;
            if (message == null)
                return;

            int argPos = 0;

            if (!(message.HasCharPrefix(Settings.Default.CmdPrefix, ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)))
                return;

            var context = new CommandContext(client, message);
            var result = await commands.ExecuteAsync(context, argPos, services);

            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
       
    }
}
