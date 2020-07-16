import { Message, Collection } from 'discord.js';
import { injectable } from 'inversify';
import { Repository } from 'mongodb-typescript';
import { Logger } from 'typescript-logging';
import container from '../../../util/inversify.config';
import { TYPES } from '../../../util/types';
import { DbClient } from '../../../util/dbclient';
import { KawaekoBotUwUCounter } from '../../../entities/kawaeko-uwu-counter';

@injectable()
export class UwUCounterService {
    async handle(message: Message) {
        if (message.guild == null) return;
        if (message.content.toLowerCase().includes('uwu') == false) return;
        var kawaekoId = `${message.guild.id}${message.author.id}`;
        var guildId = message.guild.id;
        var userId = message.author.id;
        var uwuRepo = new Repository<KawaekoBotUwUCounter>(KawaekoBotUwUCounter, container.get<DbClient>(TYPES.DbClient).db, "UwU Counter");
        await uwuRepo.findById(kawaekoId).then(async (uwuEntry) => {
            if (uwuEntry == null) {
                await uwuRepo.insert(new KawaekoBotUwUCounter(kawaekoId, guildId, userId, 1));
            } else {
                await uwuRepo.update(new KawaekoBotUwUCounter(kawaekoId, guildId, userId, uwuEntry.uwuCount + 1));
            }
        });
    };
};