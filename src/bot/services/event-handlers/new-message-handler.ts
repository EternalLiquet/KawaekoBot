import { Message, Collection } from 'discord.js';
import { injectable } from 'inversify';
import { Repository } from 'mongodb-typescript';
import { Logger } from 'typescript-logging';

@injectable()
export class NewMessageHandler {
    async handle(message: Message, commandList: Collection<string, any>) {
        var command = commandList.find(command => message.content.includes(`~> ${command.name}`));
        if (!command) command = commandList.find(command => message.content.includes(`~> ${command.alias}`));
        if (command) {
            command.execute(message, message.content.substring((`~> ${command.name}`).length, message.content.length).trim())
        }
    }
}