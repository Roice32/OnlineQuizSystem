import { QuestionBase } from "../questions";

export type QuestionBaseResult= {
    userId : string;
    questionId : string;
    score : number;
    type : QuestionBase;
}