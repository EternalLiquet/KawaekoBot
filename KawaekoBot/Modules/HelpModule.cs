using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace KawaekoBot.Modules
{
    [Name("Command Information/Help")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;

        public HelpModule(CommandService commandService)
        {
            _commandService = commandService;
        }

        [Command("help")]
        [Summary("Lists all the commands that Kawaeko Bot is able to use")]
        [Remarks("help")]
        public async Task HelpCommand()
        {
            string stringPrefix = "~> ";

            EmbedBuilder helpBuilder = new EmbedBuilder()
            {
                Title = "Kawaeko Bot Commands",
                Description = $"These are the commands that are available to you\nTo use them, type {stringPrefix} followed by any of the commands below.\n Ex: {stringPrefix}meme",
                Color = new Color(218, 112, 214),
                ThumbnailUrl = "https://cdn.discordapp.com/avatars/727571196748496976/8b5a2ccc6284786f5224e5f8155f5c14.png?size=256"
            };

            foreach (var module in _commandService.Modules)
            {
                string description = null;
                foreach (var command in module.Commands)
                {
                    var result = await command.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                    {
                        description += $"**{command.Aliases.First()}**\n";
                        if (command.Aliases.Count > 1)
                        {
                            description += command.Aliases.Count > 2 ? $"This command can also be used by using the following aliases: \n" : $"This command can also be used by using the following alias: \n";
                            foreach (var alias in command.Aliases)
                            {
                                if (alias != command.Aliases.First())
                                    description += $"\t*{alias}*\n";
                            }
                        }
                        description += $"Function: {command.Summary}\n";
                        description += $"{stringPrefix}{command.Remarks}\n";
                        description += "\n";
                    }
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    helpBuilder.AddField(field =>
                    {
                        field.Name = $"**=== {module.Name} ===**";
                        field.Value = description;
                        field.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, helpBuilder.Build());
        }
    }
}
