export class Blog{
    constructor(
        public blogID:number,
        public title:string,
        public content:string,
        public applicationUserID:number,
        public username: string,
        public publishDate:Date,
        public updateDate: Date,
        public deleteConfirmation:boolean=false,
        public photoID?:number
    ){}
}