import { QuestionBase } from "../questions";
import { QuestionBaseResult } from "./question-result-base";


export type QuizResultBody = {
    questions : QuestionBase[];
    questionResults: QuestionBaseResult[];
}