import { QuizResultBody } from "./quiz-result-body";
import { QuizResultHeader } from "./quiz-result-header";

  export type QuizResult =  {
    asQuizCreator: boolean;
    quizResultHeader: QuizResultHeader;
    quizResultBody: QuizResultBody;
  }
  