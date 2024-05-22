export interface QuizResultHeader {
    username: string;
    quizName: string;
    completionTime: string;
    score: number;
    reviewPending: boolean;
  }
  
  export interface Question {
    id: string;
    text: string;
    type: string;
  }
  
  export interface QuestionResult {
    questionId: string;
    score: number;
  }
  
  export interface QuizResultBody {
    questions: Question[];
    questionResults: QuestionResult[];
  }
  
  export interface QuizResults {
    quizResultHeaders: QuizResultHeader;
    quizResultBody: QuizResultBody;
  }
  