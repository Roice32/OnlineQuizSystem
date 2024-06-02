/* eslint-disable @typescript-eslint/no-unused-vars */
export enum QuestionType{
    TrueFalse,
    MultipleChoice,
    SingleChoice,
    WrittenAnswer,
    ReviewNeeded
}


export interface QuestionBase{
    id:string;
    text:string;
    type: QuestionType;
    choices?: string[];
}

/* export interface ChoiceQuestion extends QuestionBase{
    choices: string[];
} */





