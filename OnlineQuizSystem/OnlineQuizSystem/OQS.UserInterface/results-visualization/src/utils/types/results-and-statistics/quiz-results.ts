import { QuestionType } from "../questions";
import { AnswerResult } from "./question-review";

export type QuizResultHeader = {
    quizId: string;
    userId: string;
    username: string;
    quizName: string;
    completionTime: string;
    score: number;
    reviewPending: boolean;
  }
  
  export type Question = {
    quizId: string;
    id: string;
    text: string;
    type: QuestionType;
  }
  
  export type QuestionResult = {
    userId: string;
    questionId: string;
    score: number;
    reviewNeededResult: AnswerResult;
  }
  
  export type QuizResultBody = {
    questions: Question[];
    questionResults: QuestionResult[];
  }
  
  export type QuizResults =  {
    then(arg0: () => void): unknown;
    userId : string;
    quizId : string;
    quizResultHeader: QuizResultHeader;
    quizResultBody: QuizResultBody;
  }
  