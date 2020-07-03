import { Message } from 'discord.js';
import fetch from 'cross-fetch';
import container from "../../../../util/inversify.config";
import { Logger } from "typescript-logging";

export class FunModule {
    private FunModuleLogger: Logger;

    constructor(
        @inject(TYPES.FunModuleLogger) funModuleLogger: Logger

    ) {
        this.FunModuleLogger = funModuleLogger;
    }

    public ModuleCommandList = [
        {
            name: 'yuumi',
            description: 'Meow adc-kun let me buff u uwu',
            help_text: `POC`,
            alias: 'meow',
            async execute(message: Message, args: string) {
                message.channel.send('meow adc-kun let me buff u uwu');
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
                        console.error(error);
                    })
            }
        }
    ];
}