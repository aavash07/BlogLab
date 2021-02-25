export class ApplicationUserLogin{
    constructor(
        public applicationUserID:number,
        public username:string,
        public fullname:string,
        public email:string,
        public token:string
    ){}
}