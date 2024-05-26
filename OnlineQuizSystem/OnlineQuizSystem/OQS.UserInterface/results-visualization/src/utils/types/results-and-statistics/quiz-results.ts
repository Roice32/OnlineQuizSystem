import { QuestionType } from "../questions";

export interface QuizResultHeader {
    quizId: string;
    userId: string;
    username: string;
    quizName: string;
    completionTime: string;
    score: number;
    reviewPending: boolean;
  }
  
  export interface Question {
    quizId: string;
    id: string;
    text: string;
    type: QuestionType;
  }
  
  export interface QuestionResult {
    userId: string;
    questionId: string;
    score: number;
  }
  
  export interface QuizResultBody {
    questions: Question[];
    questionResults: QuestionResult[];
  }
  
  export interface QuizResults {
    then(arg0: () => void): unknown;
    userId : string;
    quizId : string;
    quizResultHeader?: QuizResultHeader;
    quizResultBody: QuizResultBody;
  }
  