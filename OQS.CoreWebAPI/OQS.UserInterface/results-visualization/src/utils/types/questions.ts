/* eslint-disable @typescript-eslint/no-unused-vars */
export enum QuestionType{
    TrueFalse,
    MultipleChoice,
    SingleChoice,
    WriteAnswer,
    ReviewNeeded
}


export interface QuestionBase{
    id:string;
    text:string;
    type: string;
    choices?: string[];
}

/* export interface ChoiceQuestion extends QuestionBase{
    choices: string[];
} */





