export class Photo{
    constructor(
        public photoID: number,
        public applicationUserID:number,
        public imageUrl:string,
        public publicID:string,
        public description: string,
        public publishDate: Date,
        public updateDate: Date,
        public deletionConfirm : boolean=false
    ){}
}