import {  QuestionType } from "./questions";
import { NakedQuiz } from "./naked-quiz";

export type ActiveQuiz = {
    id: string;
    deadline: string;
    quiz: NakedQuiz;
    answers: Map<string,Answer>;
}

export type Answer={
    questionId:string;
    questionType:QuestionType;
    trueFalseAnswer?:boolean;
    multipleChoiceAnswers?:string[];
    singleChoiceAnswer?:string;
    writeAnswer?:string;
    reviewNeeded?:string;
}
