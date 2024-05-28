import { QuestionType } from "../questions";
import { AnswerResult } from "./question-review";

export type QuizResultHeader = {
    quizId: string;
    userId: string;
    userName: string;
    quizName: string;
    completionTime: string;
    score: number;
    reviewPending: boolean;
  }
  
  export type Question = {
    id: string;
    quizId: string;
    type: QuestionType;
    text: string;
    allocatedPoints: number;

    choices?: string[];
    trueFalseAnswer?: boolean;
    multipleChoiceAnswers?: string[];
    singleChoiceAnswer?: string;
    writtenAcceptedAnswers?: string[];
  }
  
  export type QuestionResult = {
    userId: string;
    questionId: string;
    score: number;
    type: QuestionType;

    trueFalseAnswerResult?: AnswerResult;

    pseudoDictionaryChoicesResults?: string;

    writtenAnswer?: string;
    writtenAnswerResult?: AnswerResult;

    reviewNeededAnswer?: string;
    LLMReview?: string;
    reviewNeededResult?: AnswerResult;
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
