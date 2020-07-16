import { Message } from 'discord.js';
import fetch from 'cross-fetch';
import { KawaekoBotUwUCounter } from '../../../../entities/kawaeko-uwu-counter';
import { DbClient } from '../../../../util/dbclient';
import container from "../../../../util/inversify.config";
import { Logger } from "typescript-logging";
import { Repository } from 'mongodb-typescript';
import { TYPES } from '../../../../util/types';

export class FunModule {
    public ModuleCommandList = [
        {
            name: 'yuumi',
            description: 'Meow adc-kun let me buff u uwu',
            help_text: `POC`,
            alias: 'meow',
            async execute(message: Message, args: string) {
                message.channel.send('meow adc-kun let me buff u uwu')
                .catch(error => {
                    Promise.reject(error);
                });
            }
        },
        {
            name: '8ball',
            description: 'Let\'s predict the future',
            help_text: `POC`,
            alias: 'meow',
            async execute(message: Message, args: string) {
                if (args.trim() == '') message.channel.send('Please ask a question');

                var uri = `https://8ball.delegator.com/magic/JSON/${encodeURIComponent(args)}`
                await fetch(uri)
                    .then(response => response.json())
                    .then(answer => message.channel.send(answer.magic.answer))
                    .catch(error => {
                        Promise.reject(error);
                    })
            }
        },
        {
            name: 'meme',
            description: 'Give random meme',
            help_text: 'POC',
            alias: 'meme',
            async execute(message: Message, args: string) {
                var uri = `https://meme-api.herokuapp.com/gimme`;
                console.log('here')
                await fetch(uri)
                    .then(response => response.json())
                    .then(answer => { console.log(answer.url)
                        message.channel.send(answer.url)})
                    .catch(error => {
                        Promise.reject(error);
                    })
            }
        }, 
        {
            name: 'uwu',
            description: 'See how many times a user has said uwu',
            help_text: 'POC',
            alias: 'owo',
            async execute(message: Message, args: string) {
                if (args.trim() == '') {
                    var uwuRepo = new Repository<KawaekoBotUwUCounter>(KawaekoBotUwUCounter, container.get<DbClient>(TYPES.DbClient).db, "UwU Counter");
                    var uwuList = uwuRepo.find({});
                    var uwuCount = 0;
                    await uwuList.forEach((entry) => {
                        if(entry.guildId == message.guild.id) {
                            uwuCount += entry.uwuCount;
                        };
                    });
                    message.channel.send(`${message.guild.name}'s uwu count is ${uwuCount}`);
                } else if(message.guild.members.cache.find(user => user.displayName.trim().toLowerCase() == args.trim().toLowerCase())) {
                    var guildMember = message.guild.members.cache.find(user => user.displayName.trim().toLowerCase() == args.trim().toLowerCase());
                    if(guildMember.user.bot) { 
                        message.reply(`${guildMember.displayName} is a bot`);
                        return;
                    }
                    var uwuRepo = new Repository<KawaekoBotUwUCounter>(KawaekoBotUwUCounter, container.get<DbClient>(TYPES.DbClient).db, "UwU Counter");
                    var uwuList = uwuRepo.find({});
                    var uwuCount = 0;
                    await uwuList.forEach((entry) => {
                        if(entry.guildId == message.guild.id && entry.userId == guildMember.id) {
                            uwuCount += entry.uwuCount;
                        };
                    });
                    message.channel.send(`${guildMember.displayName}'s uwu count is ${uwuCount}`)
                } else {
                    message.reply(`user ${args} not found!`);
                };
            }
        }
    ];
}