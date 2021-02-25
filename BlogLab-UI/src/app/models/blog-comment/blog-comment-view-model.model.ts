export class BlogCommentViewModel{
    constructor(
        public parentBlogCommentID :number,
        public blogCommentID:number,
        public blogID:number,
        public content:string,
        public username:string,
        public applicationUserID:number,
        public publisdhDate:Date,
        public updateDate: Date,
        public isEditable: boolean=false,
        public deleteConfirm: boolean= false,
        public isReplying: boolean=false,
        public comments: BlogCommentViewModel[],
    ){}
}