export class BlogCommentCreate{
    constructor(
        public blogCommentID:number,
        public blogID:string,
        public content:string,
        public parentBlogCommentID? :number,
        
    ){}
}