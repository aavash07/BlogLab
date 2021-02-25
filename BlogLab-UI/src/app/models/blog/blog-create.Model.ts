export class BlogCreate{
    constructor(
        public blogID:number,
        public title:string,
        public content:string,
        public photoID:number
    ){}
}