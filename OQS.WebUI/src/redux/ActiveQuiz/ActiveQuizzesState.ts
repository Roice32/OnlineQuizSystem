/* eslint-disable @typescript-eslint/no-unused-vars */
import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { ActiveQuiz, Answer } from "../../utils/types/active-quiz";

export interface ActiveQuizState {
  activeQuiz: ActiveQuiz;
  currentQuestion: number;
}

export interface SubmitAnswerPayload {
  activeQuizId: string;
  answer: Answer;
}

export interface ChangePagePayload{
  activeQuizId: string;
}

const initialState: ActiveQuizState[] = [];

const activeQuizSlice = createSlice({
  name: "activeQuizzes",
  initialState,
  reducers: {
    addActiveQuiz: (state, action: PayloadAction<ActiveQuiz>) => {
      state.push({ activeQuiz: action.payload, currentQuestion: 0 });
    },
    submitAnswer: (state, action: PayloadAction<SubmitAnswerPayload>) => {
      const { activeQuizId, answer } = action.payload;
      const { activeQuiz, currentQuestion } = state.find(
        (state) => state.activeQuiz.id === activeQuizId
      ) as ActiveQuizState;
      if (
        activeQuiz.quiz.questions.some(
          (question) => question.id === answer.questionId
        )
      )
        activeQuiz.answers[answer.questionId] = answer;
    },
    nextQuestion: (state,action:PayloadAction<ChangePagePayload>) => {
      const { activeQuizId } = action.payload;
      state.filter((quizState) => quizState.activeQuiz.id === activeQuizId).forEach((quizState) => {
        quizState.currentQuestion += 1;
      });
    },

    previousQuestion: (state,action:PayloadAction<ChangePagePayload>) => {
      const { activeQuizId } = action.payload;
      state.filter((quizState) => quizState.activeQuiz.id === activeQuizId).forEach((quizState) => {
        quizState.currentQuestion -= 1;
      });
    },
    goToFirstQuestion: (state,action:PayloadAction<ChangePagePayload>) => {
      const { activeQuizId } = action.payload;
      state.filter((quizState) => quizState.activeQuiz.id === activeQuizId).forEach((quizState) => {
        quizState.currentQuestion =0;
      });
    },
    goToLastQuestion: (state,action:PayloadAction<ChangePagePayload>) => {
      const { activeQuizId } = action.payload;
      state.filter((quizState) => quizState.activeQuiz.id === activeQuizId).forEach((quizState) => {
        quizState.currentQuestion = quizState.activeQuiz.quiz.questions.length - 1;
      });
    },

    deleteActiveQuiz: (state, action: PayloadAction<string>) => {
      return state.filter((quizState) => quizState.activeQuiz.id !== action.payload);
    },

    clearExpiredActiveQuizzes: (state) => {
      const now=new Date();
      /* console.log(now); */
      return state.filter((quizState) => new Date(quizState.activeQuiz.deadline) > now);
    }
  },
});

export const {
  addActiveQuiz,
  submitAnswer,
  nextQuestion,
  previousQuestion,
  deleteActiveQuiz,
  goToFirstQuestion,
  goToLastQuestion,
  clearExpiredActiveQuizzes
} = activeQuizSlice.actions;

export default activeQuizSlice.reducer;
