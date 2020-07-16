import { id } from 'mongodb-typescript';

export class KawaekoBotUwUCounter {
    @id kawaekoBotId: string;
    guildId: string;
    userId: string;
    uwuCount: number;

    constructor(kawaekoBotId: string, guildId: string, userId: string, uwuCount: number) {
        this.kawaekoBotId = kawaekoBotId;
        this.guildId = guildId;
        this.userId = userId;
        this.uwuCount = uwuCount;
    }
}