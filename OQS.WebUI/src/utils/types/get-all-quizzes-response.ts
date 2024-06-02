import { pagination } from "./pagination";
import { Quiz } from "./quiz";

export type GetAllQuizzesResponse = {
    quizzes: Quiz[];
    pagination: pagination;
}
