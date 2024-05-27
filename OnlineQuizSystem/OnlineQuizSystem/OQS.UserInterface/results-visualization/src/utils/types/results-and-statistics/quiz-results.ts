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
    trueFalseAnswerResult: AnswerResult;
    multipleChoiceResult: AnswerResult;
    singleChoiceResult: AnswerResult;
    writtenAnswerResult: AnswerResult;
    writtenAnswer: string;
    reviewNeededAnswer : string;
    LLMReview : string;
    multipleChoiceAnswers : string[];
    
    singleChoiceAnswer : string;
    pseudoDictionaryChoicesResults: string;
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
  