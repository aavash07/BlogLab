export class BlogComment{
    constructor(
        public blogCommentID:number,
        public blogID:string,
        public content:string,
        public uername:string,
        public applicationUserID:string,
        public publisdhDate:Date,
        public updateDate: Date,
        public parentBlogCommentID? :number,
    ){}
}