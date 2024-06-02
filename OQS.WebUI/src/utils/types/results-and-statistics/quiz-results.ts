import { QuizResultBody } from "./quiz-result-body";
import { QuizResultHeader } from "./quiz-result-header";

  export type QuizResult =  {
    userId : string;
    quizId : string;
    quizResultHeader: QuizResultHeader;
    quizResultBody: QuizResultBody;
  }
  